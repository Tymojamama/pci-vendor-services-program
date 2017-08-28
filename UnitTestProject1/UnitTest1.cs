using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCI.VSP.Data.CRM.Model;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using System.Web.Services.Protocols;
using PCI.VSP.Data.CRM.DataLogic;
using PCI.VSP.Data.Enums;
using PCI.VSP.Data.Classes;
using PCI.VSP.Services;
using PCI.VSP.Web;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            System.Security.SecureString ss = new System.Security.SecureString();
            foreach (Char c in ("TomatoWork643%").ToCharArray())
            {
                ss.AppendChar(c);
            }
            ss.MakeReadOnly();

            PCI.VSP.Data.CrmServiceSettings crmServiceSettings = new PCI.VSP.Data.CrmServiceSettings()
            {
                DeploymentType = 2,
                DomainName = "Pension1",
                OrganizationName = "TestEnvironment",
                Password = ss,
                ServiceUrl = "https://dev.pension-consultants.com/MSCRMServices/2007/SPLA/CrmDiscoveryService.asmx",
                Username = "zallen"
            };

            PCI.VSP.Data.Globals.Initialize(crmServiceSettings);
            IAuthenticationRequest _authRequest = new AuthenticationRequest()
            {
                Username = PCI.VSP.Data.Globals.CrmServiceSettings.Username,
                Password = PCI.VSP.Data.Globals.CrmServiceSettings.Password
            };

            VendorQuestionDataLogicTest test = new VendorQuestionDataLogicTest(_authRequest);
            test.calculate();
        }
    }

    class VendorQuestionDataLogicTest : ServiceObjectBase<VendorQuestion, Guid>
    {
        private static String[] _columnSet = new String[] { "vsp_categoryid", "vsp_name", "vsp_questiondatatypeid", "vsp_questionid", "vsp_templateid", "vsp_vendorid", 
            "vsp_vendorquestionid", "vsp_vendorwording", "vsp_vendorproductid, vsp_clientquestionid" };
        public const String _entityName = "vsp_vendorquestion";

        public VendorQuestionDataLogicTest(IAuthenticationRequest authRequest) : base(authRequest, _entityName, null) { }

        public void calculate()
        {
            Guid clientProjectID = new Guid("20838758-E824-E511-941B-00155D288102");
           

            List<Template> usedTemplates = new TemplateDataLogic(_authRequest).RetrieveTemplatesUsedByClientProject(clientProjectID, TemplateType.VendorMonitoringInvestmentCompany);

            var clientQuestions = new ClientQuestionDataLogic(_authRequest).Retrieve(clientProjectID, QuestionTypes.VendorMonitoring, usedTemplates.Select(z => z.Id).ToList());
            List<Guid> clientQuestionIds = clientQuestions.Select(z => z.Id).ToList();

//            string fetchXml =
//    @"
//<fetch version='1.0' output-format='xml-platform' mapping='logical'>
//    <entity name='vsp_vendorquestion'>
//        <all-attributes />
//        <link-entity name='vsp_clientquestion' from='vsp_clientquestionid' to='vsp_clientquestionid' alias='cq' link-type='inner'>
//            <attribute name='vsp_clientquestionid' />
//            <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t2' link-type='inner'>
//                <attribute name='vsp_templateid' />
//                <attribute name='vsp_name' />
//                <order attribute='vsp_name' descending='false' />
//            </link-entity>
//            <link-entity name='vsp_templatequestion' from='vsp_questionid' to='vsp_vspquestionid' alias='tq' link-type='inner'>
//                <attribute name='vsp_sortorder' />
//                <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t1' link-type='inner'>
//                    <attribute name='vsp_templateid' />
//                </link-entity>
//                <order attribute='vsp_sortorder' descending='false' />
//            </link-entity>
//        </link-entity>
//        <filter type='and'>
//            <condition attribute='vsp_vendorid' operator='eq' value='5d26d75d-bb1b-e311-8545-d8d385c29901' />
//            <condition attribute='vsp_clientquestionid' operator='in'><value>4c838758-e824-e511-941b-00155d288102</value>
//<value>67838758-e824-e511-941b-00155d288102</value>
//<value>7e717d5e-e824-e511-941b-00155d288102</value>
//<value>99717d5e-e824-e511-941b-00155d288102</value>
//<value>b4717d5e-e824-e511-941b-00155d288102</value>
//<value>cf717d5e-e824-e511-941b-00155d288102</value>
//<value>ea717d5e-e824-e511-941b-00155d288102</value>
//<value>92197864-e824-e511-941b-00155d288102</value>
//<value>ad197864-e824-e511-941b-00155d288102</value>
//<value>c8197864-e824-e511-941b-00155d288102</value>
//<value>e3197864-e824-e511-941b-00155d288102</value>
//<value>fe197864-e824-e511-941b-00155d288102</value>
//<value>3053756a-e824-e511-941b-00155d288102</value>
//<value>4b53756a-e824-e511-941b-00155d288102</value>
//<value>6653756a-e824-e511-941b-00155d288102</value></condition>
//            <condition attribute='vsp_categoryid' operator='eq' value='47d95f0c-92ed-e211-9e06-d8d385c29901' />
//            <condition attribute='vsp_questionfunctionid' operator='eq' value='15bcb861-92ed-e211-9e06-d8d385c29901' />
//            <condition attribute='vsp_vendormonitoringanswertype' operator='eq' value='20002' />
//            <condition attribute='vsp_serviceproviderid' operator='eq' value='654332c5-fa30-e311-b2ca-d8d385c29901' />
//        </filter>
//    </entity>
//</fetch>";

            string fetchXml =
@"
<fetch version='1.0' output-format='xml-platform' mapping='logical'>
    <entity name='vsp_vendorquestion'>
        <all-attributes />
        <link-entity name='vsp_clientquestion' from='vsp_clientquestionid' to='vsp_clientquestionid' alias='cq' link-type='outer'>
            <attribute name='vsp_clientquestionid' />
            <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t2' link-type='outer'>
                <attribute name='vsp_templateid' />
                <attribute name='vsp_name' />
                <order attribute='vsp_name' descending='false' />
            </link-entity>
            <link-entity name='vsp_templatequestion' from='vsp_questionid' to='vsp_vspquestionid' alias='tq' link-type='outer'>
                <attribute name='vsp_sortorder' />
                <link-entity name='vsp_template' from='vsp_templateid' to='vsp_templateid' alias='t1' link-type='outer'>
                    <attribute name='vsp_templateid' />
                </link-entity>
                <order attribute='vsp_sortorder' descending='false' />
            </link-entity>
        </link-entity>
        <filter type='and'>
            <condition attribute='vsp_vendorid' operator='eq' value='7cfb2fd1-1dd5-e211-a403-d8d385c29901' />
            <condition attribute='vsp_clientquestionid' operator='in'><value>4c838758-e824-e511-941b-00155d288102</value>
<value>67838758-e824-e511-941b-00155d288102</value>
<value>7e717d5e-e824-e511-941b-00155d288102</value>
<value>99717d5e-e824-e511-941b-00155d288102</value>
<value>b4717d5e-e824-e511-941b-00155d288102</value>
<value>cf717d5e-e824-e511-941b-00155d288102</value>
<value>ea717d5e-e824-e511-941b-00155d288102</value>
<value>92197864-e824-e511-941b-00155d288102</value>
<value>ad197864-e824-e511-941b-00155d288102</value>
<value>c8197864-e824-e511-941b-00155d288102</value>
<value>e3197864-e824-e511-941b-00155d288102</value>
<value>fe197864-e824-e511-941b-00155d288102</value>
<value>3053756a-e824-e511-941b-00155d288102</value>
<value>4b53756a-e824-e511-941b-00155d288102</value>
<value>6653756a-e824-e511-941b-00155d288102</value></condition>
            <condition attribute='vsp_categoryid' operator='eq' value='47d95f0c-92ed-e211-9e06-d8d385c29901' />
            <condition attribute='vsp_questionfunctionid' operator='eq' value='15bcb861-92ed-e211-9e06-d8d385c29901' />
            <condition attribute='vsp_vendormonitoringanswertype' operator='eq' value='20002' />
            <condition attribute='vsp_serviceproviderid' operator='eq' value='2ccca1e2-5eb5-404c-ae3a-d5e243a7763b' />
        </filter>
    </entity>
</fetch>";

            var results = base.Fetch(fetchXml).Descendants("result")
                .Select(el => new PCI.VSP.Data.CRM.Model.VendorQuestion(el))
                //.Where(vq => vq.Attributes["t1.vsp_templateid"].Equals(vq.Attributes["t2.vsp_templateid"]))
                .ToList();

            
            var a = 0;
        }
    }
}
