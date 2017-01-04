using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.Sdk.Query;

namespace PCI.VSP.Plugins
{
    public class VSPQuestionChangePlugin : IPlugin
    {
        private const string _entityName = "vsp_question";
        private const string _questionid = "vsp_questionid";
        private const string _vspQuestionImage = "VSPQuestionUpdate";
        private const string _vendorWording = "vsp_vendorwording";
        private const string _choiceAnswers = "vsp_choiceanswers";
        private const string _questionDataType = "vsp_questiondatatype";
        private const string _questionType = "vsp_questiontype";

        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity entity = null;
            try
            {
#if DEBUG
                TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(@"C:\Temp\PCI.VSP.Plugins.VSPQuestionChangePlugin.log"));
                Trace.Listeners.Add(tl);
#endif

                #region Retrieve and Parse Input Parameters

                Trace.WriteLine("context.InputParameters.Properties:");
                foreach (PropertyBagEntry pbe in context.InputParameters.Properties)
                    Trace.WriteLine("Name: " + pbe.Name + "; Value: " + pbe.Value.ToString());
                Trace.WriteLine(String.Empty);

                // Check if the InputParameters property bag contains a target of the current operation and that target is of type DynamicEntity.
                if (context.InputParameters.Properties.Contains(ParameterName.Target) && context.InputParameters.Properties[ParameterName.Target] is DynamicEntity)
                {
                    // Obtain the target business entity from the input parmameters.
                    entity = (DynamicEntity)context.InputParameters.Properties[ParameterName.Target];

                    // Test for an entity type and message supported by the plug-in.
                    Trace.WriteLine("Message name: " + context.MessageName);
                    Trace.WriteLine("Entity name: " + entity.Name);
                    Trace.WriteLine(String.Empty);
                    Trace.Flush();
                    if (entity.Name != _entityName) { return; }
                    switch (context.MessageName)
                    {
                        case MessageName.Create:
                        case MessageName.Update:
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    return;
                }

                #endregion

                #region Retrieve and Parse Post-Image

                DynamicEntity image = null;
                if (context.PostEntityImages.Contains(_vspQuestionImage))
                    image = (DynamicEntity)context.PostEntityImages[_vspQuestionImage];
                
                if (image != null)
                {
                    Trace.WriteLine("Image name: " + image.Name);
                    Trace.WriteLine("Image properties:");

                    foreach (Property p in image.Properties)
                        Trace.WriteLine(p.Name);
                    Trace.WriteLine(String.Empty);
                    Trace.Flush();
                }

                #endregion

                Guid vspQuestionId = Guid.Empty;
                string vendorWording = null;
                string choiceAnswers = null;
                string questionDataType = null;
                string questionType = null;

                #region Parse out the VSP Question ID

                if (image != null)
                {
                    if (!image.Properties.Contains(_questionid) || image.Properties[_questionid] == null || String.IsNullOrEmpty(image.Properties[_questionid].ToString()))
                        return;
                    vspQuestionId = ((Key)image.Properties[_questionid]).Value;

                    if (image.Properties.Contains(_vendorWording) && image.Properties[_vendorWording] != null)
                        vendorWording = (string)image.Properties[_vendorWording];

                    if (image.Properties.Contains(_choiceAnswers) && image.Properties[_choiceAnswers] != null)
                        choiceAnswers = (string)image.Properties[_choiceAnswers];

                    if (image.Properties.Contains(_questionDataType) && image.Properties[_questionDataType] != null)
                        questionDataType = ((Picklist)image.Properties[_questionDataType]).Value.ToString();

                    if (image.Properties.Contains(_questionType) && image.Properties[_questionType] != null)
                        questionType = ((Picklist)image.Properties[_questionType]).Value.ToString();
                }
                else
                {
                    if (!entity.Properties.Contains(_questionid) || entity.Properties[_questionid] == null || String.IsNullOrEmpty(entity.Properties[_questionid].ToString()))
                        return;
                    vspQuestionId = ((Key)entity.Properties[_questionid]).Value;

                    if (entity.Properties.Contains(_vendorWording) && entity.Properties[_vendorWording] != null)
                        vendorWording = (string)entity.Properties[_vendorWording];

                    if (entity.Properties.Contains(_choiceAnswers) && entity.Properties[_choiceAnswers] != null)
                        choiceAnswers = (string)entity.Properties[_choiceAnswers];

                    if (entity.Properties.Contains(_questionDataType) && entity.Properties[_questionDataType] != null)
                        questionDataType = ((Picklist)entity.Properties[_questionDataType]).Value.ToString();

                    if (entity.Properties.Contains(_questionType) && entity.Properties[_questionType] != null)
                        questionType = ((Picklist)entity.Properties[_questionType]).Value.ToString();
                }
                
                Trace.WriteLine("VSP Question Id: " + vspQuestionId);
                Trace.WriteLine("Vendor Wording: " + (vendorWording == null ? "NULL" : vendorWording));
                Trace.WriteLine("Choice Answers: " + (choiceAnswers == null ? "NULL" : choiceAnswers));
                Trace.WriteLine("Question Data Type: " + (questionDataType == null ? "NULL" : questionDataType));
                Trace.WriteLine("Question Type: " + (questionType == null ? "NULL" : questionType));
                
                #endregion

                if (vspQuestionId != Guid.Empty)
                {
                    #region Update and Invalidate Vendor Questions

                    ICrmService crmService = context.CreateCrmService(true);
                    var vqdl = new DataLogic.VendorQuestionDataLogic(crmService);
                    var vql = vqdl.RetrieveByVSPQuestionId(vspQuestionId);
                    Trace.WriteLine("Updating " + vql.Count() + " Vendor Questions.");
                    Trace.WriteLine(string.Empty);

                    foreach (var vq in vql)
                    {
                        Trace.WriteLine("Processing Vendor Question Id: " + vq.Id);

                        vq.InvalidAnswerReason = Model.Enums.InvalidAnswerReasons.WordingChange;

                        Trace.WriteLine("BEFORE");
                        Trace.WriteLine("Vendor Wording: " + vq.VendorWording);
                        Trace.WriteLine("Choice Answers: " + vq.ChoiceAnswers);
                        Trace.WriteLine("Question Data Type: " + vq.QuestionDataType.ToString());
                        Trace.WriteLine("Question Type: " + vq.QuestionType.ToString());

                        if (vendorWording != null) vq.VendorWording = vendorWording;
                        if (choiceAnswers != null) vq.ChoiceAnswers = choiceAnswers;

                        if (questionDataType != null)
                        {
                            var qdt = Enum.Parse(typeof(Model.Enums.DataTypes), questionDataType);
                            if (qdt != null)
                                vq.QuestionDataType = (Model.Enums.DataTypes)qdt;
                        }

                        if (questionType != null)
                        {
                            var qt = Enum.Parse(typeof(Model.Enums.QuestionTypes), questionType);
                            if (qt != null)
                                vq.QuestionType = (Model.Enums.QuestionTypes)qt;
                        }

                        Trace.WriteLine(string.Empty);
                        Trace.WriteLine("AFTER");
                        Trace.WriteLine("Vendor Wording: " + vq.VendorWording);
                        Trace.WriteLine("Choice Answers: " + vq.ChoiceAnswers);
                        Trace.WriteLine("Question Data Type: " + vq.QuestionDataType.ToString());
                        Trace.WriteLine("Question Type: " + vq.QuestionType.ToString());

                        vqdl.Save(vq, Guid.Empty);
                        Trace.WriteLine("Updated Vendor Question Id: " + vq.Id);
                        Trace.WriteLine(string.Empty);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Exception and Inner Exception Logging

                Trace.WriteLine("EXCEPTION: " + ex.Message);
                Trace.WriteLine("Stack: " + ex.StackTrace);

                if (ex.GetType() == typeof(System.Web.Services.Protocols.SoapException))
                {
                    Trace.WriteLine("Detail: ");
                    Trace.WriteLine(((System.Web.Services.Protocols.SoapException)ex).Detail.InnerText);
                }

                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    Trace.WriteLine(" ");
                    Trace.WriteLine("INNER EXCEPTION: " + innerException.Message);
                    Trace.WriteLine("Stack: " + innerException.StackTrace);
                    innerException = innerException.InnerException;
                }

                #endregion
                throw;
            }
            finally
            {
                Trace.Flush();
                Trace.Close();
            }
        }
    }
}
