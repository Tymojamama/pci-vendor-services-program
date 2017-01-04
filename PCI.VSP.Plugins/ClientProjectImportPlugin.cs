using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

// Microsoft Dynamics CRM namespaces
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Crm.SdkTypeProxy.Metadata;
using PCI.VSP.Plugins.Model;
using PCI.VSP.Plugins.DataLogic;
using System.IO;

namespace PCI.VSP.Plugins
{
    // 1. Get templateQuestion, branch on trigger type
    // 2. Get questions from templateQuestions
    // 3. Add questions to clientProject that don't already exist
    //      a. get all questions with clientProjectId
    //      b. add questions that don't already exist

    public class ClientProjectImportPlugin : IPlugin
    {
        private const String _entityName = "vsp_clientproject";
        private const String _importclientprojectid = "vsp_importclientprojectid";
        private const String _importTemplateId = "vsp_importtemplateid";
        private const String _importClientAccountId = "vsp_clientaccountid";
        private const String _id = "vsp_clientprojectid";

        private enum ImportTypes
        {
            Unspecified = 0,
            ClientProject = 1,
            Template = 2
        }

        private void Filter(List<Model.Question> sourceQs, List<Model.Question> targetQs)
        {
            if (sourceQs == null || targetQs == null) { return; }
            List<Model.Question> xects = (from targetQ in targetQs
                                          join sourceQ in sourceQs on targetQ.QuestionId equals sourceQ.QuestionId
                                          select targetQ).ToList();

            foreach (Model.Question xect in xects)
                sourceQs.Remove(xect);
        }

        private void Filter(List<Model.ClientQuestion> sourceQs, List<Model.ClientQuestion> targetQs)
        {
            if (sourceQs == null || targetQs == null) { return; }
            List<Model.ClientQuestion> xects = (from targetQ in targetQs
                                          join sourceQ in sourceQs on targetQ.QuestionId equals sourceQ.QuestionId
                                          select targetQ).ToList();

            foreach (Model.ClientQuestion xect in xects)
                sourceQs.Remove(xect);
        }

        private void ImportClientInquiries(ICrmService crmService, ImportTypes importType, Guid sourceEntityId, Guid targetClientProjectId, Guid clientId)
        {
            Trace.WriteLine("Entered ImportClientInquiries()");
            DataLogic.QuestionDataLogic qdl = new DataLogic.QuestionDataLogic(crmService);
            List<Model.Question> sourceQs = null;

            switch (importType)
            {
                case ImportTypes.ClientProject:
                    Trace.WriteLine("Retrieving questions by clientProjectId " + sourceEntityId.ToString());
                    sourceQs = qdl.RetrieveByClientProject(sourceEntityId);
                    break;
                //case ImportTypes.Template:
                //    Trace.WriteLine("Retrieving questions by templateId " + sourceEntityId.ToString());
                //    sourceQs = qdl.RetrieveByTemplateId(sourceEntityId);
                //    break;
                default:
                    break;
            }
            if (sourceQs == null || sourceQs.Count == 0) { return; }

            Trace.WriteLine("Retrieving existing questions associated to clientProjectId " + targetClientProjectId.ToString());
            List<Model.Question> targetQs = qdl.RetrieveByClientProject(targetClientProjectId);

            Trace.WriteLine("Filtering existing questions out of questions to add.");
            Filter(sourceQs, targetQs);

            // save the new questions
            Trace.WriteLine("Creating " + sourceQs.Count.ToString() + " questions associated to clientProjectId " + targetClientProjectId.ToString());
            DataLogic.ClientQuestionDataLogic cqdl = new DataLogic.ClientQuestionDataLogic(crmService);
            foreach (Model.Question sourceQ in sourceQs)
            {
                Model.ClientQuestion clientQuestion = new Model.ClientQuestion(sourceQ);
                clientQuestion.ClientProjectId = targetClientProjectId;
                clientQuestion.ClientId = clientId;
                clientQuestion.Id = cqdl.Create(clientQuestion);
            }
        }

        //private void ImportClientInquiries(ICrmService crmService, ImportTypes importType, Guid sourceEntityId, Guid targetClientProjectId)
        //{
        //    Trace.WriteLine("Entered ImportClientInquiries()");
        //    DataLogic.QuestionDataLogic qdl = new DataLogic.QuestionDataLogic(crmService);
        //    List<Model.Question> sourceQs = null;

        //    switch (importType)
        //    {
        //        case ImportTypes.ClientProject:
        //            Trace.WriteLine("Retrieving questions by clientProjectId " + sourceEntityId.ToString());
        //            sourceQs = qdl.RetrieveByClientProject(sourceEntityId);
        //            break;
        //        case ImportTypes.Template:
        //            Trace.WriteLine("Retrieving questions by templateId " + sourceEntityId.ToString());
        //            sourceQs = qdl.RetrieveByTemplateId(sourceEntityId);
        //            break;
        //        default:
        //            break;
        //    }
        //    if (sourceQs == null || sourceQs.Count == 0) { return; }

        //    Trace.WriteLine("Retrieving existing questions associated to clientProjectId " + targetClientProjectId.ToString());
        //    List<Model.Question> targetQs = qdl.RetrieveByClientProject(targetClientProjectId);

        //    Trace.WriteLine("Filtering existing questions out of questions to add.");
        //    Filter(sourceQs, targetQs);

        //    // save the new questions
        //    Trace.WriteLine("Creating " + sourceQs.Count.ToString() + " questions associated to clientProjectId " + targetClientProjectId.ToString());
        //    foreach (Model.Question sourceQ in sourceQs)
        //    {
        //        sourceQ.QuestionId = Guid.Empty;
        //        sourceQ.ClientProjectId = targetClientProjectId;
        //        sourceQ.QuestionId = qdl.Create(sourceQ);
        //    }
        //}

        private void ImportClientQuestions(ICrmService crmService, ImportTypes importType, Guid sourceEntityId, Guid targetClientProjectId, Guid clientId)
        {
            Trace.WriteLine("Entered ImportClientQuestions()");
            DataLogic.ClientQuestionDataLogic cqdl = new DataLogic.ClientQuestionDataLogic(crmService);
            List<Model.ClientQuestion> sourceQs = null;

            switch (importType)
            {
                case ImportTypes.ClientProject:
                    Trace.WriteLine("Retrieving client questions by clientProjectId " + sourceEntityId.ToString());
                    sourceQs = cqdl.RetrieveByClientProject(sourceEntityId);
                    break;
                case ImportTypes.Template:
                    sourceQs = new List<Model.ClientQuestion>();

                    Trace.WriteLine("Retrieving template by templateId " + sourceEntityId.ToString());
                    Model.Template template = new DataLogic.TemplateDataLogic(crmService).Retrieve(sourceEntityId);

                    Trace.WriteLine("Retrieving template questions by templateId " + sourceEntityId.ToString());
                    List<TemplateQuestion> tql = new TemplateQuestionDataLogic(crmService).RetrieveByTemplateId(sourceEntityId);
                    Trace.WriteLine("Template Questions retrieved: " + tql.Count);

                    Trace.WriteLine("Retrieving client questions by templateId " + targetClientProjectId.ToString());
                    List<ClientQuestion> cql = new ClientQuestionDataLogic(crmService).RetrieveByClientProject(targetClientProjectId);
                    Trace.WriteLine("Client Questions retrieved: " + cql.Count);

                    foreach (var tq in tql)
                    {
                        //Trace.WriteLine("Check if VSP Question is already a Client Question: " + tq.QuestionId.ToString());
                        //bool alreadyExists = false;

                        //foreach (var cq in cql)
                        //{
                        //    if (cq.QuestionId == tq.QuestionId)
                        //    {
                        //        Trace.WriteLine("VSP Question already exists as Client Question: " + tq.QuestionId.ToString());
                        //        alreadyExists = true;
                        //        break;
                        //    }
                        //}

                        //if (!alreadyExists)
                        //{
                            //Trace.WriteLine("VSP Question does not exist as client question: " + tq.QuestionId.ToString());
                        Trace.WriteLine("Begin adding VSP Question to Client Question List: " + tq.QuestionId.ToString());
                        Question q = new QuestionDataLogic(crmService).Retrieve(tq.QuestionId);
                        if (q != null)
                        {
                            Model.ClientQuestion cq = new Model.ClientQuestion(q);
                            cq.ClientProjectId = targetClientProjectId;
                            cq.ClientId = clientId;
                            cq.FilterCategory = template.FilterCategory;
                            cq.CategoryId = tq.QuestionCategoryId;
                            cq.FunctionId = tq.QuestionFunctionId;
                            cq.SortOrder = tq.SortOrder;
                            cq.TemplateId = sourceEntityId; // sourceEntityId = Template ID
                            sourceQs.Add(cq);
                        }
                        else
                            Trace.WriteLine("VSP Question not found: " + tq.QuestionId.ToString());

                        Trace.WriteLine("End adding VSP Question to Client Question List: " + tq.QuestionId.ToString());
                        //}
                    }

                    //Trace.WriteLine("Retrieving questions by templateId " + sourceEntityId.ToString());
                    //DataLogic.QuestionDataLogic qdl = new DataLogic.QuestionDataLogic(crmService);
                    //List<Model.Question> ql = qdl.RetrieveByTemplate(sourceEntityId);
                    //if (ql != null)
                    //{

                    //    Model.Template template = new DataLogic.TemplateDataLogic(crmService).Retrieve(sourceEntityId);
                    //    sourceQs = new List<Model.ClientQuestion>();
                    //    foreach (Model.Question q in ql)
                    //    {
                    //        Model.ClientQuestion cq = new Model.ClientQuestion(q);
                    //        cq.ClientProjectId = targetClientProjectId;
                    //        cq.ClientId = clientId;
                    //        cq.FilterCategory = template.FilterCategory;
                    //        sourceQs.Add(cq);
                    //    }
                    //}
                    break;
                default:
                    break;
            }
            if (sourceQs == null || sourceQs.Count == 0) { return; }

            Trace.WriteLine("Retrieving existing client questions associated to clientProjectId " + targetClientProjectId.ToString());
            List<Model.ClientQuestion> targetQs = cqdl.RetrieveByClientProject(targetClientProjectId);

            Trace.WriteLine("Filtering existing questions out of questions to add.");
            Filter(sourceQs, targetQs);

            Trace.WriteLine("Creating " + sourceQs.Count.ToString() + " client questions associated to clientProjectId " + targetClientProjectId.ToString());
            foreach (Model.ClientQuestion sourceQ in sourceQs)
            {
                sourceQ.Id = Guid.Empty;
                sourceQ.ClientProjectId = targetClientProjectId;
                sourceQ.Id = cqdl.Create(sourceQ);
            }
        }

        //private void ImportClientQuestions(ICrmService crmService, ImportTypes importType, Guid sourceEntityId, Guid targetClientProjectId)
        //{
        //    Trace.WriteLine("Entered ImportClientQuestions()");
        //    DataLogic.ClientQuestionDataLogic cqdl = new DataLogic.ClientQuestionDataLogic(crmService);
        //    List<Model.ClientQuestion> sourceQs = null;

        //    switch (importType)
        //    {
        //        case ImportTypes.ClientProject:
        //            Trace.WriteLine("Retrieving client questions by clientProjectId " + sourceEntityId.ToString());
        //            sourceQs = cqdl.RetrieveByClientProject(sourceEntityId);
        //            break;
        //        default:
        //            break;
        //    }
        //    if (sourceQs == null || sourceQs.Count == 0) { return; }

        //    Trace.WriteLine("Retrieving existing client questions associated to clientProjectId " + targetClientProjectId.ToString());
        //    List<Model.ClientQuestion> targetQs = cqdl.RetrieveByClientProject(targetClientProjectId);

        //    Trace.WriteLine("Filtering existing questions out of questions to add.");
        //    Filter(sourceQs, targetQs);

        //    Trace.WriteLine("Creating " + sourceQs.Count.ToString() + " client questions associated to clientProjectId " + targetClientProjectId.ToString());
        //    foreach (Model.ClientQuestion sourceQ in sourceQs)
        //    {
        //        sourceQ.Id = Guid.Empty;
        //        sourceQ.ClientProjectId = targetClientProjectId;
        //        sourceQ.Id = cqdl.Create(sourceQ);
        //    }
        //}

        private void ImportClientInformation(ICrmService crmService, Guid sourceEntityId, Guid targetEntityId, Guid clientId)
        {
            DataLogic.ClientProjectDataLogic cpDL = new ClientProjectDataLogic(crmService);

            ClientProject sourceProject = cpDL.Retrieve(sourceEntityId);
            ClientProject targetProject = cpDL.Retrieve(targetEntityId);

            if (sourceProject != null && targetProject != null)
            {
                targetProject.Name = (sourceProject.Name ?? string.Empty) + " - Project Copy";
                targetProject.ClientProjectType = sourceProject.ClientProjectType;
                targetProject.ClientId = sourceProject.ClientId;
                targetProject.ContactId = sourceProject.ContactId;
                targetProject.ManagerId = sourceProject.ManagerId;
                targetProject.Status = sourceProject.Status;
                targetProject.StatusReason = sourceProject.StatusReason;
                targetProject.OwnerId = sourceProject.OwnerId;
                targetProject.ClientComment = sourceProject.ClientComment ?? string.Empty;
                targetProject.PciComment = sourceProject.PciComment ?? string.Empty;
                targetProject.PlanAccountId = sourceProject.PlanAccountId;

                cpDL.Update(targetProject);
            }
        }

        public void Execute(IPluginExecutionContext context)
        {
            DynamicEntity entity = null;
            try
            {
#if DEBUG
                TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(@"C:\Temp\PCI.VSP.Plugins.ClientProjectImportPlugin.log"));
                Trace.Listeners.Add(tl);
#endif

                Trace.WriteLine("context.InputParameters.Properties:");
                foreach (PropertyBagEntry pbe in context.InputParameters.Properties)
                    Trace.WriteLine("Name: " + pbe.Name + "; Value: " + pbe.Value.ToString());
                Trace.WriteLine(String.Empty);

                // Check if the InputParameters property bag contains a target
                // of the current operation and that target is of type DynamicEntity.
                if (!context.InputParameters.Properties.Contains(ParameterName.Target) || !(context.InputParameters.Properties[ParameterName.Target] is DynamicEntity))
                    return;

                // Obtain the target business entity from the input parmameters.
                entity = (DynamicEntity)context.InputParameters.Properties[ParameterName.Target];

                // Test for an entity type and message supported by the plug-in.
                Trace.WriteLine("Message name: " + context.MessageName);
                Trace.WriteLine("Entity name: " + entity.Name);
                Trace.WriteLine(String.Empty);

                if (entity.Name != _entityName) { return; }
                switch (context.MessageName)
                {
                    case MessageName.Create:
                    case MessageName.Update:
                        break;
                    default:
                        return;
                }

                Trace.WriteLine("Contains vsp_importclientprojectid: " + entity.Properties.Contains(_importclientprojectid).ToString());
                Trace.WriteLine("Contains vsp_importtemplateid: " + entity.Properties.Contains(_importTemplateId).ToString());
                
                
                Guid importClientProjectId = new Guid();
                Guid importTemplateId = new Guid();
                Guid clientId = new Guid();
                Guid clientProjectId = ((Key)entity.Properties[_id]).Value;

                Trace.WriteLine("Contains post-image ClientProjectImportImage: " + context.PostEntityImages.Contains("ClientProjectImportImage").ToString());

                if (context.PostEntityImages.Contains("ClientProjectImportImage"))
                {
                    DynamicEntity image = (DynamicEntity)context.PostEntityImages["ClientProjectImportImage"];
                    Trace.WriteLine("Contains image attribute vsp_clientaccountid: " + image.Properties.Contains("vsp_clientaccountid").ToString());
                    if (image.Properties.Contains("vsp_clientaccountid"))
                        clientId = ((Lookup)image.Properties["vsp_clientaccountid"]).Value;
                }

                if (entity.Properties.Contains(_importclientprojectid))
                    importClientProjectId = ((Lookup)entity.Properties[_importclientprojectid]).Value;

                if (entity.Properties.Contains(_importTemplateId))
                    importTemplateId = ((Lookup)entity.Properties[_importTemplateId]).Value;

                if (importClientProjectId == Guid.Empty && importTemplateId == Guid.Empty) { return; }
                
                ICrmService crmService = context.CreateCrmService(true);
                if (importClientProjectId != Guid.Empty)
                    this.ImportClientInquiries(crmService, ImportTypes.ClientProject, importClientProjectId, clientProjectId, clientId);

                if (importClientProjectId != Guid.Empty)
                    this.ImportClientQuestions(crmService, ImportTypes.ClientProject, importClientProjectId, clientProjectId, clientId);

                if (importClientProjectId != Guid.Empty)
                    this.ImportClientInformation(crmService, importClientProjectId, clientProjectId, clientId);

                if (importTemplateId != Guid.Empty)
                    this.ImportClientQuestions(crmService, ImportTypes.Template, importTemplateId, clientProjectId, clientId);

                crmService.Dispose();
            }
            catch (IOException)
            {
                // nothing
            }
            catch (Exception ex)
            {
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
