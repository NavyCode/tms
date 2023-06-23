using System.Xml.Serialization;
namespace TestPlanService.Models.Export.Navy.V1;


// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(Export));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (Export)serializer.Deserialize(reader);
// }
// using System.Xml.Serialization;
// XmlSerializer serializer = new XmlSerializer(typeof(Export));
// using (StringReader reader = new StringReader(xml))
// {
//    var test = (Export)serializer.Deserialize(reader);
// }

[XmlRoot(ElementName = "step")]
public class Step
{

    [XmlElement(ElementName = "action")]
    public string Action { get; set; }

    [XmlElement(ElementName = "result")]
    public string Result { get; set; }
}

[XmlRoot(ElementName = "config")]
public class Config
{
    [XmlElement(ElementName = "tester")]
    public string Tester { get; set; } 

    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    public Outcome Outcome { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "test")]
public class Test
{
    [XmlElement(ElementName = "assignedTo")]
    public string AssignedTo { get; set; }
    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlElement(ElementName = "priority")]
    public int Priority { get; set; }

    [XmlElement(ElementName = "state")]
    public string State { get; set; }

    [XmlElement(ElementName = "description")]
    public string Description { get; set; }

    [XmlElement(ElementName = "automationStatus")]
    public string AutomationStatus { get; set; }

    [XmlElement(ElementName = "automationTestName")]
    public string AutomationTestName { get; set; }

    [XmlElement(ElementName = "automationTestStorage")]
    public string AutomationTestStorage { get; set; }

    [XmlElement(ElementName = "automationTestType")]
    public string AutomationTestType { get; set; }

    [XmlElement(ElementName = "precondition")]
    public string Precondition { get; set; }

    [XmlElement(ElementName = "postcondition")]
    public string Postcondition { get; set; }

    [XmlElement(ElementName = "step")]
    public List<Step> Step { get; set; }

    [XmlElement(ElementName = "config")]
    public List<Config> Config { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "suite")]
public class Suite
{

    [XmlElement(ElementName = "test")]
    public List<Test> Test { get; set; }

    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlElement(ElementName = "parent")]
    public Parent Parent { get; set; }

    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }

    [XmlText]
    public string Text { get; set; }
}

[XmlRoot(ElementName = "parent")]
public class Parent
{

    [XmlAttribute(AttributeName = "id")]
    public int Id { get; set; }
}

[XmlRoot(ElementName = "export")]
public class Export
{

    [XmlElement(ElementName = "suite")]
    public List<Suite> Suite { get; set; }

    [XmlAttribute(AttributeName = "version")]
    public int Version { get; set; }

    [XmlText]
    public string Text { get; set; }
}

