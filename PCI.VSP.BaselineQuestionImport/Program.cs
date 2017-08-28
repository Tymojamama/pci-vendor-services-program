using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using PCI.VSP.Data.CRM.Model;
using PCI.VSP.Data.Enums;
using System.Diagnostics;
using PCI.VSP.Data.CRM.DataLogic;
using System.Text.RegularExpressions;

namespace PCI.VSP.BaselineQuestionImport
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            TraceListener tl = new TextWriterTraceListener(System.IO.File.CreateText(AppDomain.CurrentDomain.BaseDirectory + "BaselineQuestionImport.log"));
            Trace.Listeners.Add(tl);

            try
            {
                Trace.WriteLine("Begin Processing");
                InitializeDataLogic();

                string fullImportFilePath = ConfigurationManager.AppSettings["FullImportFilePath"];
                Trace.WriteLine("Import File Path: " + fullImportFilePath);
                bool overWriteExisting = false;
                bool.TryParse(ConfigurationManager.AppSettings["OverWriteExisting"], out overWriteExisting);
                Trace.WriteLine("Overwrite Existing: " + overWriteExisting);

                if (string.IsNullOrEmpty(fullImportFilePath)) { Console.WriteLine("Import file path is null or empty"); Console.ReadKey(); return; }
                int lineNumber = 0;

                using (StreamReader sr = new StreamReader(fullImportFilePath))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            lineNumber++;
                            string[] columns = line.Split('|');
                            if (RemoveSpecialCharacters(columns[0]) == "QuestionType") continue; // bypass header row

                            BaselineQuestionRecord baselineQuestion = new BaselineQuestionRecord();

                            QuestionTypes questionType = GetQuestionType(RemoveSpecialCharacters(columns[0]));
                            switch (questionType)
                            {
                                case QuestionTypes.SearchQuestion_Filter1:
                                    baselineQuestion = new BaselineQuestionRecord()
                                    {
                                        Question_Type = RemoveSpecialCharacters(columns[0]),
                                        Name = columns[1],
                                        Client_Wording = columns[2],
                                        Vendor_Wording = columns[3],
                                        Report_Label = columns[4],
                                        Question_Data_Type = RemoveSpecialCharacters(columns[5]),
                                        Fee_Type = RemoveSpecialCharacters(columns[6]),
                                        Client_Answer_Type = RemoveSpecialCharacters(columns[7]),
                                        Comparision_Type = RemoveSpecialCharacters(columns[8]),
                                        Vendor_Answer_Type = RemoveSpecialCharacters(columns[9]),
                                        Minimum_Allowed_Answer = columns[10],
                                        Maximum_Allowed_Answer = columns[11],
                                        Plan_Information_Visible_To_Vendor = columns[12],
                                        Choice_Answers = columns[13].Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                                        Asset_Class = columns[14],
                                        PCI_Comment_To_Client = columns[15],
                                        PCI_Comment_To_Vendor = columns[16],
                                        LegacyTableName = columns[17],
                                        LegacyTableID = columns[18]
                                    };
                                    break;
                                case QuestionTypes.PlanAssumption:
                                    baselineQuestion = new BaselineQuestionRecord()
                                    {
                                        Question_Type = RemoveSpecialCharacters(columns[0]),
                                        Name = columns[1],
                                        Client_Wording = columns[2],
                                        Vendor_Wording = columns[3],
                                        Report_Label = columns[4],
                                        Question_Data_Type = RemoveSpecialCharacters(columns[5]),
                                        Fee_Type = RemoveSpecialCharacters(columns[6]),
                                        Client_Answer_Type = RemoveSpecialCharacters(columns[7]),
                                        Comparision_Type = RemoveSpecialCharacters(columns[8]),
                                        Vendor_Answer_Type = RemoveSpecialCharacters(columns[9]),
                                        Minimum_Allowed_Answer = columns[10],
                                        Maximum_Allowed_Answer = columns[11],
                                        Choice_Answers = columns[12].Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                        //Asset_Class = columns[14],
                                    };
                                    break;
                                case QuestionTypes.Fee:
                                    baselineQuestion = new BaselineQuestionRecord()
                                    {
                                        Question_Type = RemoveSpecialCharacters(columns[0]),
                                        Name = columns[2],
                                        Client_Wording = columns[3],
                                        Vendor_Wording = columns[4],
                                        Report_Label = columns[5],
                                        Question_Data_Type = RemoveSpecialCharacters(columns[6]),
                                        Fee_Type = RemoveSpecialCharacters(columns[7]),
                                        //Client_Answer_Type = RemoveSpecialCharacters(columns[7]),
                                        //Comparision_Type = RemoveSpecialCharacters(columns[8]),
                                        Vendor_Answer_Type = RemoveSpecialCharacters(columns[8]),
                                        Minimum_Allowed_Answer = columns[1],
                                        Maximum_Allowed_Answer = columns[9],
                                        //Plan_Information_Visible_To_Vendor = columns[12],
                                        //Choice_Answers = columns[13].Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                                        //Asset_Class = columns[14],
                                        //PCI_Comment_To_Client = columns[15],
                                        //PCI_Comment_To_Vendor = columns[16],
                                        //LegacyTableName = columns[17],
                                        //LegacyTableID = columns[18]
                                    };
                                    break;
                                case QuestionTypes.SearchQuestion_Filter2:
                                    baselineQuestion = new BaselineQuestionRecord()
                                    {
                                        Question_Type = RemoveSpecialCharacters(columns[0]),
                                        Name = columns[1],
                                        Client_Wording = columns[2],
                                        Vendor_Wording = columns[3],
                                        Report_Label = columns[4],
                                        Question_Data_Type = RemoveSpecialCharacters(columns[5]),
                                        Fee_Type = RemoveSpecialCharacters(columns[6]),
                                        Client_Answer_Type = RemoveSpecialCharacters(columns[7]),
                                        Comparision_Type = RemoveSpecialCharacters(columns[8]),
                                        Vendor_Answer_Type = RemoveSpecialCharacters(columns[9]),
                                        Minimum_Allowed_Answer = columns[10],
                                        Maximum_Allowed_Answer = columns[11],
                                        Plan_Information_Visible_To_Vendor = columns[12],
                                        Choice_Answers = columns[13].Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                                        Asset_Class = columns[14],
                                        PCI_Comment_To_Client = columns[15],
                                        PCI_Comment_To_Vendor = columns[16]
                                    };
                                    break;
                            }
                            try
                            {
                                string choiceAnswers = "";
                                Guid assetId = Guid.Empty;
                                Guid planAssumtionId = Guid.Empty;
                                string[] choices;
                                QuestionDataLogic qdl = new QuestionDataLogic(GetDefaultAuthRequest());
                                InvestmentAssetClassDataLogic iadl = new InvestmentAssetClassDataLogic(GetDefaultAuthRequest());
                                List<Question> baselineQuestions = null;
                                Question oldQuestion = null;

                                switch (questionType)
                                {
                                    case QuestionTypes.SearchQuestion_Filter1:
                                    case QuestionTypes.SearchQuestion_Filter2:
                                        if (baselineQuestion.Choice_Answers != null && baselineQuestion.Choice_Answers.Length > 0)
                                        {
                                            choices = baselineQuestion.Choice_Answers[0].Split('~');
                                            foreach (string choice in choices)
                                            {
                                                choiceAnswers += choice + "\n ";
                                            }
                                        }
                                        if (overWriteExisting)
                                        {
                                            if (baselineQuestions == null)
                                            {
                                                baselineQuestions = qdl.RetrieveAll();
                                                baselineQuestions = baselineQuestions.Where(d => d.QuestionType == QuestionTypes.SearchQuestion_Filter1).ToList();
                                            }

                                            oldQuestion = baselineQuestions.Where(d => RemoveSpecialCharacters(d.Name).ToLower().Contains(RemoveSpecialCharacters(baselineQuestion.Name).ToLower())).FirstOrDefault();
                                            //if (oldQuestion != null)
                                            //{
                                            //    oldQuestion.QuestionType = GetQuestionType(baselineQuestion.Question_Type);(d.LegacyTableID == ((!string.IsNullOrEmpty(baselineQuestion.LegacyTableID)) ? Int32.Parse(baselineQuestion.LegacyTableID.Trim()) : 0) && d.LegacyTableName == baselineQuestion.LegacyTableName) ||
                                            //    oldQuestion.Name = baselineQuestion.Name;
                                            //    oldQuestion.ClientWording = baselineQuestion.Client_Wording;
                                            //    oldQuestion.VendorWording = baselineQuestion.Vendor_Wording;
                                            //    oldQuestion.ReportLabel = baselineQuestion.Report_Label;
                                            //    oldQuestion.QuestionDataType = GetQuestionDataType(baselineQuestion.Question_Data_Type);
                                            //    oldQuestion.FeeType = GetFeeType(baselineQuestion.Fee_Type);
                                            //    oldQuestion.ClientAnswerType = GetAnswerDataType(baselineQuestion.Client_Answer_Type);
                                            //    oldQuestion.ComparisonType = GetComparisonType(baselineQuestion.Comparision_Type);
                                            //    oldQuestion.VendorAnswerType = GetAnswerDataType(baselineQuestion.Vendor_Answer_Type);
                                            //    oldQuestion.MinimumAnswerAllowed = baselineQuestion.Minimum_Allowed_Answer;
                                            //    oldQuestion.MaximumAnswerAllowed = baselineQuestion.Maximum_Allowed_Answer;
                                            //    oldQuestion.PlanAssumptionVisibleToVendor = IsVisibletoVendor(baselineQuestion.Plan_Information_Visible_To_Vendor);
                                            //    oldQuestion.ChoiceAnswers = choiceAnswers;
                                            //    oldQuestion.PCICommentToClient = baselineQuestion.PCI_Comment_To_Client ?? "";
                                            //    oldQuestion.PCICommentToVendor = baselineQuestion.PCI_Comment_To_Vendor ?? "";
                                            //    oldQuestion.AssetClassId = assetId;
                                            //    oldQuestion.LegacyTableID = Int32.Parse(baselineQuestion.LegacyTableID);
                                            //    oldQuestion.LegacyTableName = baselineQuestion.LegacyTableName;
                                            //    oldQuestion.ImportDate = DateTime.Now;

                                            //    if (!string.IsNullOrEmpty(baselineQuestion.Asset_Class.Trim()))
                                            //    {
                                            //        var invests = iadl.RetrieveAll();
                                            //        var investment = invests.Where(d => RemoveSpecialCharacters(d.Name).ToLower() == RemoveSpecialCharacters(baselineQuestion.Asset_Class).ToLower()).FirstOrDefault();
                                            //        if (investment != null)
                                            //            oldQuestion.AssetClassId = investment.Id;
                                            //    }
                                            //    qdl.Delete(oldQuestion.QuestionId);
                                            //    qdl.Create(oldQuestion);
                                            //    //qdl.Update(oldQuestion);
                                            //    //CreateorUpdateQuestions(overWriteExisting, baselineQuestions, q, qdl);
                                            //}
                                            if (oldQuestion == null)
                                            {
                                                //var q = new Question()
                                                //{
                                                //    QuestionType = GetQuestionType(baselineQuestion.Question_Type),
                                                //    Name = baselineQuestion.Name,
                                                //    ClientWording = baselineQuestion.Client_Wording,
                                                //    VendorWording = baselineQuestion.Vendor_Wording,
                                                //    ReportLabel = baselineQuestion.Report_Label,
                                                //    QuestionDataType = GetQuestionDataType(baselineQuestion.Question_Data_Type),
                                                //    FeeType = GetFeeType(baselineQuestion.Fee_Type),
                                                //    ClientAnswerType = GetAnswerDataType(baselineQuestion.Client_Answer_Type),
                                                //    ComparisonType = GetComparisonType(baselineQuestion.Comparision_Type),
                                                //    VendorAnswerType = GetAnswerDataType(baselineQuestion.Vendor_Answer_Type),
                                                //    MinimumAnswerAllowed = baselineQuestion.Minimum_Allowed_Answer,
                                                //    MaximumAnswerAllowed = baselineQuestion.Maximum_Allowed_Answer,
                                                //    PlanAssumptionVisibleToVendor = IsVisibletoVendor(baselineQuestion.Plan_Information_Visible_To_Vendor),
                                                //    ChoiceAnswers = choiceAnswers,
                                                //    PCICommentToClient = baselineQuestion.PCI_Comment_To_Client ?? "",
                                                //    PCICommentToVendor = baselineQuestion.PCI_Comment_To_Vendor ?? "",
                                                //    AssetClassId = assetId,
                                                //    LegacyTableID = (!string.IsNullOrEmpty(baselineQuestion.LegacyTableID)) ? Int32.Parse(baselineQuestion.LegacyTableID.Trim()) : 0,
                                                //    LegacyTableName = baselineQuestion.LegacyTableName,
                                                //    ImportDate = DateTime.Now
                                                //};
                                                //if (!string.IsNullOrEmpty(baselineQuestion.Asset_Class.Trim()))
                                                //{
                                                //    var invests = iadl.RetrieveAll();
                                                //    var investment = invests.Where(d => RemoveSpecialCharacters(d.Name).ToLower() == RemoveSpecialCharacters(baselineQuestion.Asset_Class).ToLower()).FirstOrDefault();
                                                //    if (investment != null)
                                                //        q.AssetClassId = investment.Id;
                                                //}

                                                //qdl.Create(q);
                                            }
                                            else
                                            {
                                                oldQuestion.LegacyTableID = (!string.IsNullOrEmpty(baselineQuestion.LegacyTableID)) ? Int32.Parse(baselineQuestion.LegacyTableID.Trim()) : 0;
                                                qdl.Update(oldQuestion);
                                                //qdl.Delete(oldQuestion.QuestionId);
                                            }
                                        }
                                        //if (overWriteExisting && oldQuestion == null)
                                        //{
                                        
                                        //}
                                        break;
                                    case QuestionTypes.PlanAssumption:
                                        if (baselineQuestion.Choice_Answers != null && baselineQuestion.Choice_Answers.Length > 0)
                                        {
                                            choices = baselineQuestion.Choice_Answers[0].Split('~');
                                            foreach (string choice in choices)
                                            {
                                                choiceAnswers += choice + "\n ";
                                            }
                                        }
                                        var pq = new Question()
                                        {
                                            QuestionType = GetQuestionType(baselineQuestion.Question_Type),
                                            Name = baselineQuestion.Name,
                                            ClientWording = baselineQuestion.Client_Wording,
                                            VendorWording = baselineQuestion.Vendor_Wording,
                                            ReportLabel = baselineQuestion.Report_Label,
                                            QuestionDataType = GetQuestionDataType(baselineQuestion.Question_Data_Type),
                                            FeeType = GetFeeType(baselineQuestion.Fee_Type),
                                            ClientAnswerType = GetAnswerDataType(baselineQuestion.Client_Answer_Type),
                                            ComparisonType = GetComparisonType(baselineQuestion.Comparision_Type),
                                            VendorAnswerType = GetAnswerDataType(baselineQuestion.Vendor_Answer_Type),
                                            MinimumAnswerAllowed = baselineQuestion.Minimum_Allowed_Answer,
                                            MaximumAnswerAllowed = "",
                                            PlanAssumptionVisibleToVendor = IsVisibletoVendor(baselineQuestion.Plan_Information_Visible_To_Vendor),
                                            ChoiceAnswers = choiceAnswers,
                                            PCICommentToClient = baselineQuestion.PCI_Comment_To_Client ?? "",
                                            PCICommentToVendor = baselineQuestion.PCI_Comment_To_Vendor ?? "",
                                            AssetClassId = assetId,

                                            ImportDate = DateTime.Now
                                        };

                                        if (!string.IsNullOrEmpty(baselineQuestion.Maximum_Allowed_Answer))
                                        {
                                            baselineQuestions = qdl.RetrieveAll();
                                            var planAssumption = baselineQuestions.Where(d => RemoveSpecialCharacters(d.Name).ToLower() == RemoveSpecialCharacters(baselineQuestion.Maximum_Allowed_Answer).ToLower()).FirstOrDefault();
                                            if (planAssumption != null)
                                                pq.PlanAssumptionId = planAssumption.QuestionId;
                                        }
                                        CreateorUpdateQuestions(overWriteExisting, baselineQuestions, pq, qdl);

                                        //qdl.Create(pq);
                                        break;
                                    case QuestionTypes.Fee:
                                        if (baselineQuestion.Choice_Answers != null && baselineQuestion.Choice_Answers.Length > 0)
                                        {
                                            choices = baselineQuestion.Choice_Answers[0].Split('~');
                                            foreach (string choice in choices)
                                            {
                                                choiceAnswers += choice + "\n ";
                                            }
                                        }
                                        var fq = new Question()
                                        {
                                            QuestionType = GetQuestionType(baselineQuestion.Question_Type),
                                            Name = baselineQuestion.Name,
                                            ClientWording = baselineQuestion.Client_Wording,
                                            VendorWording = baselineQuestion.Vendor_Wording,
                                            ReportLabel = baselineQuestion.Report_Label,
                                            QuestionDataType = GetQuestionDataType(baselineQuestion.Question_Data_Type),
                                            FeeType = GetFeeType(baselineQuestion.Fee_Type),
                                            ClientAnswerType = GetAnswerDataType(baselineQuestion.Client_Answer_Type),
                                            ComparisonType = GetComparisonType(baselineQuestion.Comparision_Type),
                                            VendorAnswerType = GetAnswerDataType(baselineQuestion.Vendor_Answer_Type),
                                            MinimumAnswerAllowed = "",
                                            MaximumAnswerAllowed = "",
                                            PlanAssumptionVisibleToVendor = IsVisibletoVendor(baselineQuestion.Plan_Information_Visible_To_Vendor),
                                            ChoiceAnswers = choiceAnswers,
                                            PCICommentToClient = baselineQuestion.PCI_Comment_To_Client ?? "",
                                            PCICommentToVendor = baselineQuestion.PCI_Comment_To_Vendor ?? "",
                                            //AssetClassId = assetId,
                                            ImportDate = DateTime.Now
                                        };

                                        if (!string.IsNullOrEmpty(baselineQuestion.Maximum_Allowed_Answer))
                                        {
                                            baselineQuestions = qdl.RetrieveAll();
                                            var planAssumption = baselineQuestions.Where(d => RemoveSpecialCharacters(d.Name).ToLower() == RemoveSpecialCharacters(baselineQuestion.Maximum_Allowed_Answer).ToLower()).FirstOrDefault();
                                            if (planAssumption != null)
                                                fq.PlanAssumptionId = planAssumption.QuestionId;
                                        }
                                        CreateorUpdateQuestions(overWriteExisting, baselineQuestions, fq, qdl);

                                        break;
                                    case QuestionTypes.InvestmentAssumption:
                                        break;
                                }

                                //if (overWriteExisting)
                                //{

                                //}
                                //else
                                //{

                                //}
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(string.Empty);
                                Trace.WriteLine("Error while importing question at line number: " + lineNumber);
                                Trace.WriteLine("Exception Message:" + ex.Message);
                                Trace.WriteLine("Exception Stack Trace: " + ex.Message);
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(string.Empty);
                            Trace.WriteLine("Error while processing question at line number: " + lineNumber);
                            Trace.WriteLine("Exception Message:" + ex.Message);
                            Trace.WriteLine("Exception Stack Trace: " + ex.Message);
                            continue;
                        }
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

        internal static void CreateorUpdateQuestions(bool overWriteExisting, List<Question> baselineQuestions, Question q, QuestionDataLogic qdl)
        {
            Question oldQuestion = null;

            if (overWriteExisting)
            {
                if (baselineQuestions == null)
                    baselineQuestions = qdl.RetrieveAll();


                oldQuestion = baselineQuestions.Where(d => d.LegacyTableID == q.LegacyTableID || d.LegacyTableName == q.LegacyTableName || RemoveSpecialCharacters(d.Name).ToLower() == RemoveSpecialCharacters(q.Name).ToLower()).FirstOrDefault();
                if (oldQuestion != null)
                {
                    //oldQuestion.Name = q.Name;
                    //oldQuestion.ClientWording = q.ClientWording;
                    //oldQuestion.VendorWording = q.VendorWording;
                    //oldQuestion.ReportLabel = q.ReportLabel;
                    //oldQuestion.QuestionDataType = q.QuestionDataType;
                    //oldQuestion.FeeType = q.FeeType;
                    //oldQuestion.ClientAnswerType = q.ClientAnswerType;
                    //oldQuestion.ComparisonType = q.ComparisonType;
                    //oldQuestion.VendorAnswerType = q.VendorAnswerType;
                    //oldQuestion.MinimumAnswerAllowed = q.MinimumAnswerAllowed;
                    //oldQuestion.MaximumAnswerAllowed = q.MaximumAnswerAllowed;
                    //oldQuestion.PlanAssumptionVisibleToVendor = q.PlanAssumptionVisibleToVendor;
                    //oldQuestion.PCICommentToClient = q.PCICommentToClient;
                    //oldQuestion.PCICommentToVendor = q.PCICommentToVendor;
                    //oldQuestion.AssetClassId = q.AssetClassId;
                    //oldQuestion.LegacyTableID = q.LegacyTableID;
                    //oldQuestion.LegacyTableName = q.LegacyTableName;

                    //oldQuestion.ImportDate = DateTime.Now;
                    q.QuestionId = oldQuestion.QuestionId;
                    //q.Name = oldQuestion.Name;
                    //q.ClientWording = oldQuestion.ClientWording;
                    //q.VendorWording = oldQuestion.VendorWording;
                    //q.ReportLabel = oldQuestion.ReportLabel;
                    //q.QuestionDataType = oldQuestion.QuestionDataType;
                    //q.FeeType = oldQuestion.FeeType;
                    //q.ClientAnswerType = oldQuestion.ClientAnswerType;
                    //q.ComparisonType = oldQuestion.ComparisonType;
                    //q.VendorAnswerType = oldQuestion.VendorAnswerType;
                    //q.MinimumAnswerAllowed = oldQuestion.MinimumAnswerAllowed;
                    //q.MaximumAnswerAllowed = oldQuestion.MaximumAnswerAllowed;
                    //q.PlanAssumptionVisibleToVendor = oldQuestion.PlanAssumptionVisibleToVendor;
                    //q.PCICommentToClient = oldQuestion.PCICommentToClient;
                    //q.PCICommentToVendor = oldQuestion.PCICommentToVendor;
                    //q.AssetClassId = oldQuestion.AssetClassId;
                    //q.LegacyTableID = oldQuestion.LegacyTableID;
                    //q.LegacyTableName = oldQuestion.LegacyTableName;

                    oldQuestion.ImportDate = DateTime.Now;
                    //qdl.Update(oldQuestion);
                    //qdl.Create(q);
                }
                else
                    qdl.Create(q);
            }
            else
                qdl.Create(q);
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
