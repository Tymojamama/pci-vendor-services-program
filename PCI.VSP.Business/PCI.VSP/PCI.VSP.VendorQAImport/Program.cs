using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.VendorQAImport.LegacyData;

namespace PCI.VSP.VendorQAImport
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(AppDomain.CurrentDomain.BaseDirectory + "VendorQuestionAnswerImport.log"));
            Trace.Listeners.Add(tl);

            try
            {
                Trace.WriteLine("Begin Processing");
                InitializeDataLogic();
                int questionNumber = 0;
                string accountName = "";
                string templatestoImport = ConfigurationManager.AppSettings["Templates"];
                Trace.WriteLine("Import File Path: " + templatestoImport);
                bool overWriteExisting = false;
                bool.TryParse(ConfigurationManager.AppSettings["OverWriteExisting"], out overWriteExisting);

                if (string.IsNullOrEmpty(templatestoImport)) { Console.WriteLine("Import file path is null or empty"); Console.ReadKey(); return; }


                string[] columns = templatestoImport.Split(',');
                if (columns.Count() > 0)
                {
                    try
                    {
                        VSMLegacyDataLogic vsmdl = new VSMLegacyDataLogic();
                        TemplateDataLogic tdl = new TemplateDataLogic(GetDefaultAuthRequest());
                        AccountDataLogic adl = new AccountDataLogic(GetDefaultAuthRequest());
                        TemplateQuestionDataLogic tqdl = new TemplateQuestionDataLogic(GetDefaultAuthRequest());
                        QuestionDataLogic qdl = new QuestionDataLogic(GetDefaultAuthRequest());
                        VendorQuestionDataLogic vqdl = new VendorQuestionDataLogic(GetDefaultAuthRequest());

                        List<Template> templates = tdl.RetrieveAllTemplates();
                        List<Account> accounts = adl.RetrieveAllAccounts();
                        List<VendorAnswer> allLegacyVendorAnswers = vsmdl.RetrieveAllAnswers();
                        List<PCI.VSP.VendorQAImport.LegacyData.VendorQuestion> allLegacyVendorQuestions = vsmdl.RetrieveAll();
                        List<PCI.VSP.VendorQAImport.LegacyData.VendorProduct> allLegacyVendorProducts = vsmdl.RetrieveAllVendorProducts();

                        if (templates != null && templates.Count > 0 && accounts != null && accounts.Count > 0)
                        {
                            foreach (Template template in templates)
                            {
                                if (columns.Contains(template.Name) && accounts != null && accounts.Count > 0) // == "DC Product Profile" || template.Name == "Vendor Profile"
                                {
                                    List<TemplateQuestion> templatequestions = tqdl.RetrieveByTemplateId(template.Id);

                                    foreach (Account account in accounts)
                                    {
                                        List<PCI.VSP.Data.CRM.Model.VendorProduct> vendorproducts = new VendorProductDataLogic(GetDefaultAuthRequest()).RetrieveVendorProductByVendor(account.Id);

                                        if (vendorproducts != null && vendorproducts.Count > 0)
                                            foreach (PCI.VSP.Data.CRM.Model.VendorProduct vendorproduct in vendorproducts)
                                            {
                                                List<PCI.VSP.Data.CRM.Model.VendorQuestion> allVendorQuestions = vqdl.RetrieveVendorQuestions(account.Id, null, template.Id, vendorproduct.VendorProductId);
                                                var _legacyvendorproduct = allLegacyVendorProducts.Where(d => d.VendorProductID == vendorproduct.LegacyProductID).FirstOrDefault();
                                                questionNumber = 0;
                                                if (_legacyvendorproduct != null)
                                                    foreach (TemplateQuestion templatequestion in templatequestions)
                                                    {
                                                        Question vspquestion = qdl.Retrieve(templatequestion.QuestionId);
                                                        if (vspquestion != null)
                                                        {
                                                            var _legacyQuestion = allLegacyVendorQuestions.Where(d => d.VendorQuestionID == vspquestion.LegacyTableID).FirstOrDefault();
                                                            var _legacyAnswer = allLegacyVendorAnswers.Where(d => d.VendorProductID == vendorproduct.LegacyProductID && d.VendorQuestionID == vspquestion.LegacyTableID).FirstOrDefault();

                                                            if (_legacyQuestion != null && (_legacyAnswer != null && _legacyAnswer.Answer1 != null))
                                                            {
                                                                questionNumber++;
                                                                accountName = account.Name;
                                                                var oldVendorQuestion = allVendorQuestions.Where(d => RemoveSpecialCharacters(d.Name).ToLower() == RemoveSpecialCharacters(templatequestion.Name).ToLower()).FirstOrDefault();

                                                                try
                                                                {
                                                                    if (oldVendorQuestion == null)
                                                                    {

                                                                        Trace.WriteLine("Creating Vendor Question: " + questionNumber + " for Client: " + account.Name);

                                                                        PCI.VSP.Data.CRM.Model.VendorQuestion vq = new Data.CRM.Model.VendorQuestion()
                                                                        {
                                                                            VendorId = account.Id,
                                                                            VendorProductId = vendorproduct.VendorProductId,
                                                                            VendorWording = vspquestion.VendorWording,
                                                                            VendorCommentToPCI = _legacyAnswer.VendorCommentToPCI ?? "",
                                                                            VendorCommentToClient = "",
                                                                            TemplateId = template.Id,
                                                                            Status = AccountQuestionStatuses.Answered,
                                                                            //SortOrder = 
                                                                            RevenueSharingVendorComment = "",
                                                                            RevenueSharingPciComment = "",
                                                                            RevenueSharingIsAvailable = true,
                                                                            RevenueSharingDocumentation = "",
                                                                            RevenueSharingCalculationType = RevenueSharingCalculationTypes.Unspecified,
                                                                            RevenueSharingAlternativeAvailable = false,
                                                                            QuestionType = vspquestion.QuestionType,
                                                                            QuestionId = vspquestion.QuestionId,
                                                                            QuestionDataType = vspquestion.QuestionDataType,
                                                                            PlanAssumptionId = vspquestion.PlanAssumptionId,
                                                                            PCICommentToVendor = _legacyAnswer.PCICommentToVendor ?? "",
                                                                            Name = templatequestion.Name,
                                                                            OtherRevenueSharing = 0,
                                                                            MinimumAnswerAllowed = vspquestion.MinimumAnswerAllowed,
                                                                            MaximumAnswerAllowed = vspquestion.MaximumAnswerAllowed,
                                                                            LastUpdated = DateTime.Now,
                                                                            InvestmentAssumptionsConfirmed = false,
                                                                            InvalidAnswerReason = InvalidAnswerReasons.Unspecified,
                                                                            Id = Guid.NewGuid(),
                                                                            FunctionId = templatequestion.QuestionFunctionId,
                                                                            FeeType = vspquestion.FeeType,
                                                                            //DocumentTemplateId=
                                                                            AlternateAnswer = _legacyAnswer.AnswerBeforeImport ?? "",
                                                                            AnnualContributions = 0,
                                                                            Answer = _legacyAnswer.Answer1,
                                                                            AnswerRejectedReason = "",
                                                                            AnswerType = vspquestion.VendorAnswerType,
                                                                            AssetBasedAdministrativeFee = 0,
                                                                            AssetClassId = vspquestion.AssetClassId,
                                                                            AssetFund = vspquestion.AssetFund,
                                                                            Assets = vspquestion.Assets,
                                                                            AssetSymbol = vspquestion.AssetSymbol,
                                                                            AttributeDataType = vspquestion.AttributeDataType,
                                                                            AttributeName = vspquestion.AttributeName,
                                                                            CategoryId = templatequestion.QuestionCategoryId,
                                                                            ChoiceAnswers = vspquestion.ChoiceAnswers,
                                                                            DocumentTemplateId = vspquestion.DocumentTemplateId
                                                                        };
                                                                        vqdl.Create(vq);
                                                                        Trace.WriteLine("Created Vendor Question: " + questionNumber + " for Client: " + account.Name);

                                                                    }
                                                                    else
                                                                    {
                                                                        oldVendorQuestion.Answer = _legacyAnswer.Answer1;
                                                                        oldVendorQuestion.PCICommentToVendor = _legacyAnswer.PCICommentToVendor ?? "";
                                                                        oldVendorQuestion.VendorCommentToPCI = _legacyAnswer.VendorCommentToPCI ?? "";
                                                                        vqdl.Update(oldVendorQuestion);
                                                                        Trace.WriteLine("Updated Vendor Question: " + questionNumber + " for Client: " + account.Name);
                                                                    }
                                                                }
                                                                catch (Exception qe)
                                                                {
                                                                    Trace.WriteLine("Error while updating Vendor Question: " + questionNumber + " for Client: " + accountName);
                                                                    Trace.WriteLine("Exception Message:" + qe.Message);
                                                                    Trace.WriteLine("Exception Stack Trace: " + qe.Message);
                                                                    //Trace.WriteLine("Exception Stack Trace: " + qe.);
                                                                }
                                                            }
                                                        }

                                                    }
                                            }
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error while updating Vendor Question: " + questionNumber + " for Client: " + accountName);
                        Trace.WriteLine("Exception Message:" + ex.Message);
                        Trace.WriteLine("Exception Stack Trace: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Empty);
                Trace.WriteLine("Application error");
                Trace.WriteLine("Exception Message:" + ex.Message);
                Trace.WriteLine("Exception Stack Trace: " + ex.Message);
            }
            finally
            {
                Trace.Flush();
                Trace.Close();
            }
        }

        internal static string RemoveSpecialCharacters(string questiontype)
        {
            return Regex.Replace(questiontype, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        internal static QuestionTypes GetQuestionType(string strQuestionType)
        {
            switch (strQuestionType)
            {
                case "SearchQuestionFilter1":
                    return QuestionTypes.SearchQuestion_Filter1;
                case "PlanAssumptionFilter1":
                    return QuestionTypes.PlanAssumption;
                case "Fee":
                    return QuestionTypes.Fee;
                case "SearchQuestionFilter2":
                    return QuestionTypes.SearchQuestion_Filter2;
                default:
                    return QuestionTypes.SearchQuestion_Filter1;
            }

        }

        internal static FeeType GetFeeType(string strFeetype)
        {
            switch (strFeetype)
            {
                case "":
                case null:
                    return FeeType.Unspecified;
                default:
                    return (Data.Enums.FeeType)Enum.Parse(typeof(Data.Enums.FeeType), strFeetype);
            }
        }
        internal static DataTypes GetQuestionDataType(string strDataType)
        {
            switch (strDataType)
            {
                case "YesNo":
                    return DataTypes.Yes_No;
                case "":
                case null:
                    return DataTypes.Unspecified;
                default:
                    return (DataTypes)Enum.Parse(typeof(DataTypes), strDataType);
            }
        }

        internal static AnswerTypes GetAnswerDataType(string strDataType)
        {
            switch (strDataType)
            {
                case "":
                case null:
                    return AnswerTypes.Unspecified;
                default:
                    return (AnswerTypes)Enum.Parse(typeof(AnswerTypes), strDataType);
            }
        }

        internal static Question.ComparisonTypes GetComparisonType(string strComparisonType)
        {
            string stringLowerCase = "";
            if (!string.IsNullOrEmpty(strComparisonType))
                stringLowerCase = strComparisonType.ToLower();

            switch (stringLowerCase)
            {
                case "":
                case null:
                    return Question.ComparisonTypes.Unspecified;
                case "greaterthanorequalto":
                    return Question.ComparisonTypes.GreaterThanorEqualTo;
                case "lessthanorequalto":
                    return Question.ComparisonTypes.LessThanOrEqualTo;
                default:
                    return (Question.ComparisonTypes)Enum.Parse(typeof(Question.ComparisonTypes), strComparisonType);
            }
        }

        internal static bool IsVisibletoVendor(string strVisibletovendor)
        {
            if (string.IsNullOrEmpty(strVisibletovendor))
                return false;
            else
                return bool.Parse(strVisibletovendor);
        }

        internal static void InitializeDataLogic()
        {
            Trace.WriteLine("Entering InitializeDataLogic");
            System.Security.SecureString ss = new System.Security.SecureString();
            foreach (Char c in Properties.Settings.Default.CRMPassword.ToCharArray())
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();

            Trace.WriteLine("DeploymentType: " + Properties.Settings.Default.CRMDeploymentType);
            Trace.WriteLine("DomainName: " + Properties.Settings.Default.CRMDomain);
            Trace.WriteLine("OrganizationName: " + Properties.Settings.Default.CRMOrganization);
            Trace.WriteLine("ServiceUrl: " + Properties.Settings.Default.CRMUrl);
            Trace.WriteLine("Username: " + Properties.Settings.Default.CRMUsername);
            PCI.VSP.Data.CrmServiceSettings crmServiceSettings = new PCI.VSP.Data.CrmServiceSettings()
            {
                DeploymentType = Properties.Settings.Default.CRMDeploymentType,
                DomainName = Properties.Settings.Default.CRMDomain,
                OrganizationName = Properties.Settings.Default.CRMOrganization,
                Password = ss,
                ServiceUrl = Properties.Settings.Default.CRMUrl,
                Username = Properties.Settings.Default.CRMUsername
            };

            Tricension.Data.CRM4.CrmServiceSettings crmSettings = new Tricension.Data.CRM4.CrmServiceSettings()
            {
                DeploymentType = Properties.Settings.Default.CRMDeploymentType,
                DomainName = Properties.Settings.Default.CRMDomain,
                OrganizationName = Properties.Settings.Default.CRMOrganization,
                Password = ss,
                ServiceUrl = Properties.Settings.Default.CRMUrl,
                Username = Properties.Settings.Default.CRMUsername
            };

            Tricension.Data.CRM4.Globals.Initialize(crmSettings);
            PCI.VSP.Data.Globals.Initialize(crmServiceSettings);
            Trace.WriteLine("Exiting InitializeDataLogic");
        }

        internal static PCI.VSP.Data.CRM.Model.IAuthenticationRequest GetDefaultAuthRequest()
        {
            return new AuthenticationRequest()
            {
                Username = Tricension.Data.CRM4.Globals.CrmServiceSettings.Username,
                Password = Tricension.Data.CRM4.Globals.CrmServiceSettings.Password
            };
        }

        internal static Tricension.Data.CRM4.Model.IAuthenticationRequest GetVendorDefaultAuthRequest()
        {
            return new VAuthenticationRequest()
            {
                Username = Tricension.Data.CRM4.Globals.CrmServiceSettings.Username,
                Password = Tricension.Data.CRM4.Globals.CrmServiceSettings.Password
            };
        }
    }

    internal class BaselineQuestionRecord
    {
        internal string Question_Type { get; set; }
        internal string Name { get; set; }
        internal string Client_Wording { get; set; }
        internal string Vendor_Wording { get; set; }
        internal string Report_Label { get; set; }
        internal string Question_Data_Type { get; set; }
        internal string Fee_Type { get; set; }
        internal string Client_Answer_Type { get; set; }
        internal string Comparision_Type { get; set; }
        internal string Vendor_Answer_Type { get; set; }
        internal string Minimum_Allowed_Answer { get; set; }
        internal string Maximum_Allowed_Answer { get; set; }
        internal string Plan_Information_Visible_To_Vendor { get; set; }
        internal string[] Choice_Answers { get; set; }
        internal string Asset_Class { get; set; }
        internal string PCI_Comment_To_Client { get; set; }
        internal string PCI_Comment_To_Vendor { get; set; }
        internal string LegacyTableName { get; set; }
        internal string LegacyTableID { get; set; }
    }

    internal class AuthenticationRequest : PCI.VSP.Data.CRM.Model.IAuthenticationRequest
    {
        public string Username { get; set; }
        public System.Security.SecureString Password { get; set; }
        public string DomainName { get; set; }
        public string CrmTicket { get; set; }
        public string OrganizationName { get; set; }
        public bool WasRefreshed { get; set; }
    }

    internal class VAuthenticationRequest : Tricension.Data.CRM4.Model.IAuthenticationRequest
    {
        public string Username { get; set; }
        public System.Security.SecureString Password { get; set; }
        public string DomainName { get; set; }
        public string CrmTicket { get; set; }
        public string OrganizationName { get; set; }
        public bool WasRefreshed { get; set; }
    }
}
