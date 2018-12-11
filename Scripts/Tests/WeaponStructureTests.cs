using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using XOAProductions.WeaponDesigner;

public class WeaponStructureTests {

    [Test]
    public void WeaponStructureTestsSimplePasses() {
        // Use the Assert class to test conditions.
    }

    public Object[] createTestWeaponStructure()
    {
        GameObject testGameObject = new GameObject();
        testGameObject.name = "TESTGAMEOBJECT";
        GameObject testGameObject1 = new GameObject();
        testGameObject1.name = "TESTGAMEOBJECT";
        GameObject testGameObject2 = new GameObject();
        testGameObject2.name = "TESTGAMEOBJECT";
        GameObject testGameObject3 = new GameObject();
        testGameObject3.name = "TESTGAMEOBJECT";
        GameObject testGameObject4 = new GameObject();
        testGameObject4.name = "TESTGAMEOBJECT";


        WeaponPart notSearchedPart = testGameObject1.AddComponent<WeaponPart>();
        WeaponPart notSearchedPart2 = testGameObject2.AddComponent<WeaponPart>();
        WeaponPart notSearchedPart3 = testGameObject3.AddComponent<WeaponPart>();
        WeaponPart searchedPart = testGameObject4.AddComponent<WeaponPart>();

        Adaptor adaptorLoadingMechanism = testGameObject1.AddComponent<Adaptor>();
        Adaptor adaptorLoadingMechnaism2 = testGameObject1.AddComponent<Adaptor>();
        Adaptor adaptorFiringMechanism = testGameObject2.AddComponent<Adaptor>();
        Adaptor adaptorBarrel = testGameObject4.AddComponent<Adaptor>();

        adaptorFiringMechanism.ChildPartTransform = adaptorFiringMechanism.transform;
        adaptorLoadingMechanism.ChildPartTransform = adaptorLoadingMechanism.transform;
        adaptorLoadingMechnaism2.ChildPartTransform = adaptorLoadingMechanism.transform;
        adaptorBarrel.ChildPartTransform = adaptorBarrel.transform;
        
        adaptorFiringMechanism.WeaponTypeOfAdaptor = WeaponPartType.FiringMechanism;
        adaptorLoadingMechanism.WeaponTypeOfAdaptor = WeaponPartType.LoadingMechanism;
        adaptorLoadingMechnaism2.WeaponTypeOfAdaptor = WeaponPartType.LoadingMechanism;
        adaptorBarrel.WeaponTypeOfAdaptor = WeaponPartType.Barrel;


        notSearchedPart.Adaptors = new List<Adaptor> { adaptorLoadingMechanism, adaptorLoadingMechnaism2 };
        notSearchedPart2.Adaptors = new List<Adaptor> { adaptorFiringMechanism };
        searchedPart.Adaptors = new List<Adaptor> { adaptorBarrel };

        notSearchedPart.TestSetup("NotSearchedPart", WeaponPartType.Trigger, null);
        notSearchedPart2.TestSetup("NotSearchedPart2", WeaponPartType.LoadingMechanism, notSearchedPart);
        notSearchedPart3.TestSetup("NotSearchedPart3", WeaponPartType.LoadingMechanism, notSearchedPart);
        searchedPart.TestSetup("searchedPart", WeaponPartType.FiringMechanism, notSearchedPart2);



        notSearchedPart.AddChild(notSearchedPart2);
        notSearchedPart.AddChild(notSearchedPart3);
        notSearchedPart2.AddChild(searchedPart);

        

        notSearchedPart.ConnectChildToAdaptor(adaptorLoadingMechanism, notSearchedPart2);
        notSearchedPart.ConnectChildToAdaptor(adaptorLoadingMechnaism2, notSearchedPart3);
        notSearchedPart2.ConnectChildToAdaptor(adaptorFiringMechanism, searchedPart);

        WeaponStructure weaponStructure = testGameObject.AddComponent<WeaponStructure>();
        weaponStructure.trigger = notSearchedPart;

        testGameObject1.transform.parent = testGameObject.transform;
        testGameObject2.transform.parent = testGameObject1.transform;
        testGameObject3.transform.parent = testGameObject1.transform;
        testGameObject4.transform.parent = testGameObject2.transform;

        Object[] result = new Object[5];
        result[0] = weaponStructure;
        result[1] = searchedPart;
        result[2] = notSearchedPart;
        result[3] = notSearchedPart2;
        result[4] = notSearchedPart3;

        return result;
    }

    [UnityTest]
    public IEnumerator _Adds_Part_To_Adaptor_Correctly()
    {
        var testData = createTestWeaponStructure();

        yield return null;

        WeaponStructure weaponStructure = (WeaponStructure)testData[0];
        WeaponPart firingMechanism = (WeaponPart)testData[1];
        Adaptor adaptor = firingMechanism.Adaptors[0];

        GameObject testGameObject = new GameObject();
        testGameObject.name = "TESTGAMEOBJECT";

        WeaponPart newPart = testGameObject.AddComponent<WeaponPart>();
        newPart.Adaptors = new List<Adaptor>();
        newPart.TestSetup("NewPart", WeaponPartType.Barrel, null);

        weaponStructure.AddWeaponPart(adaptor, newPart);

        Assert.Contains(newPart, firingMechanism.Children);
        Assert.AreSame(newPart.transform.parent, adaptor.ChildPartTransform);

    }


    [UnityTest]
    public IEnumerator _Removes_Part_Correctly()
    {
        var testData = createTestWeaponStructure();

        yield return null;

        WeaponPart removedPart = (WeaponPart)testData[1];
        WeaponStructure weaponStructure = (WeaponStructure)testData[0];
        WeaponPart parent = removedPart.Parent;

        removedPart.gameObject.name = "REMOVEDPART";

        weaponStructure.RemoveWeaponPart(removedPart);
        yield return null;

        Assert.That(parent.Children, Has.No.Member(removedPart));

        var go = GameObject.Find("REMOVEDPART");

        Assert.IsNull(go);

        
    }

    [UnityTest]
    public IEnumerator _Replaces_WeaponPart_Correctly()
    {
        var testData = createTestWeaponStructure();

        yield return null;

        WeaponPart replacedPart = (WeaponPart)testData[1];
        WeaponStructure weaponStructure = (WeaponStructure)testData[0];

        GameObject testGameObject = new GameObject();
        testGameObject.name = "TESTGAMEOBJECT";

        WeaponPart replacementPart = testGameObject.AddComponent<WeaponPart>();
        replacementPart.Adaptors = new List<Adaptor>();
        replacementPart.TestSetup("ReplacementPart", WeaponPartType.FiringMechanism, null);

        WeaponPart parent = replacedPart.Parent;

        weaponStructure.ReplaceWeaponPart(replacedPart, replacementPart);
        
        yield return null;

        Assert.AreSame(replacementPart, parent.Children[0]);
        Assert.Contains(replacementPart, parent.Children);
        
        Assert.AreSame(replacementPart.transform.parent, parent.Adaptors[0].ChildPartTransform);
        
    }

    [UnityTest]
    public IEnumerator _Replaces_TopLevel_WeaponPart_Correctly()
    {
        var testData = createTestWeaponStructure();

        yield return null;

        WeaponPart replacedPart = (WeaponPart)testData[2];
        WeaponStructure weaponStructure = (WeaponStructure)testData[0];

        GameObject testGameObject = new GameObject();
        testGameObject.name = "TESTGAMEOBJECT";

        WeaponPart replacementPart = testGameObject.AddComponent<WeaponPart>();

        Adaptor adaptorLoadingMechanism     = testGameObject.AddComponent<Adaptor>();
        Adaptor adaptorLoadingMechnaism2    = testGameObject.AddComponent<Adaptor>();
        adaptorLoadingMechanism.ChildPartTransform = adaptorLoadingMechanism.transform;
        adaptorLoadingMechnaism2.ChildPartTransform = adaptorLoadingMechnaism2.transform;
        adaptorLoadingMechanism.WeaponTypeOfAdaptor = WeaponPartType.LoadingMechanism;
        adaptorLoadingMechnaism2.WeaponTypeOfAdaptor = WeaponPartType.LoadingMechanism;

        replacementPart.Adaptors = new List<Adaptor> { adaptorLoadingMechanism, adaptorLoadingMechnaism2 };
        replacementPart.TestSetup("ReplacementPart", WeaponPartType.Trigger, null);

        WeaponPart parent = replacedPart.Parent;
        
        weaponStructure.ReplaceWeaponPart(replacedPart, replacementPart);

        yield return null;
        
        Assert.AreSame(((WeaponPart)testData[3]).Parent, replacementPart); //child's parent is replaced correctly
        Assert.Contains((WeaponPart)testData[3], replacementPart.Children);
        Assert.AreSame(((WeaponPart)testData[4]).Parent, replacementPart); //child's parent is replaced correctly
        Assert.Contains((WeaponPart)testData[4], replacementPart.Children);

       
    }

    [UnityTest]
    public IEnumerator _Finds_All_Parts_Of_Type_Or_Returns_Empty_List()
    {
        var testData = createTestWeaponStructure();

        yield return null;
        WeaponStructure weaponStructure = (WeaponStructure)testData[0];

        WeaponPart loadingMechanism = (WeaponPart)testData[3];
        WeaponPart loadingMechanism2 = (WeaponPart)testData[4];

        var result = weaponStructure.FindAllWeaponPartsOfTypeRecursively(WeaponPartType.LoadingMechanism);

        Assert.Contains(loadingMechanism, result);
        Assert.Contains(loadingMechanism2, result);

        Assert.AreEqual(result.Count, 2);

        result = weaponStructure.FindAllWeaponPartsOfTypeRecursively(WeaponPartType.EnergyContainer);

        Assert.AreEqual(result.Count, 0);

    }

    [UnityTest]
    public IEnumerator _Finds_WeaponPart_In_Tree_Or_Returns_Null() {
       
        

        var testData = createTestWeaponStructure();

        yield return null;

        WeaponStructure weaponStructure = (WeaponStructure)testData[0];
        WeaponPart searchedPart = (WeaponPart)testData[1];

        WeaponPart foundPart = weaponStructure.FindWeaponPartRecursive(x => x == searchedPart);
        WeaponPart notFoundPart = weaponStructure.FindWeaponPartRecursive(x => x.PartName == "LDSKFLSDLSDLLL");
        WeaponPart foundFromAlternateStart = weaponStructure.FindWeaponPartRecursive(x => x == searchedPart, foundPart.Parent);

        yield return new WaitForSeconds(1);

        Assert.AreSame(searchedPart, foundPart);

        Assert.AreEqual(null, notFoundPart);

        Assert.AreSame(searchedPart, foundFromAlternateStart);

    }

    [UnityTest]
    public IEnumerator _Finds_All_Unconnected_Adaptors()
    {
        GameObject testGameObject = new GameObject();
        testGameObject.name = "TESTGAMEOBJECT";
        GameObject testGameObject1 = new GameObject();
        testGameObject1.name = "TESTGAMEOBJECT";

        WeaponPart testPart = testGameObject1.AddComponent<WeaponPart>();

        Adaptor adaptorLoadingMechanism = testGameObject1.AddComponent<Adaptor>();
        Adaptor adaptorLoadingMechnaism2 = testGameObject1.AddComponent<Adaptor>();
        testPart.Adaptors = new List<Adaptor> { adaptorLoadingMechanism, adaptorLoadingMechnaism2 };
        
        
        testPart.TestSetup("NotSearchedPart", WeaponPartType.Trigger, null);

        WeaponStructure weaponStructure = testGameObject.AddComponent<WeaponStructure>();
        weaponStructure.trigger = testPart;
        testGameObject1.transform.parent = testGameObject.transform;
        
        yield return null;

        List<Adaptor> result = weaponStructure.FindUnconnectedAdaptorsRecursively();

        Assert.Contains(adaptorLoadingMechanism, result);
        Assert.Contains(adaptorLoadingMechnaism2, result);
        Assert.AreEqual(2, result.Count);

    }

    [TearDown]
    public void AfterEveryTest()
    {
        var gameobject = GameObject.Find("TESTGAMEOBJECT");

        if (gameobject != null)
            Object.Destroy(gameobject);
    }
}
