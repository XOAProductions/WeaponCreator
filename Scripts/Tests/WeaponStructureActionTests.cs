﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using XOAProductions.WeaponDesigner;

public class WeaponStructureActionTests {


    private List<GameObject> teardowngameobjects = new List<GameObject>();

    [Test]
    public void WeaponStructureActionTestsSimplePasses() {
        // Use the Assert class to test conditions.
    }

    public GameObject[] loadTestObjects()
    {
        List<GameObject> testObjects = new List<GameObject>();

        GameObject lamp = new GameObject();
        var light = lamp.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1;
        lamp.transform.rotation = Quaternion.Euler(0, -121, 0);

        GameObject go = new GameObject();
        var weapon = go.AddComponent<Weapon>();
        var structure = go.AddComponent<WeaponStructure>();

       

        weapon.Construct("test", structure);

        testObjects.Add(go);
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/Triggers/_k5kxxqrjl")) as GameObject); //Trigger;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/LoadingMechanisms/_zzh1k1eiq")) as GameObject); //Loader;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/FiringMechanisms/_shbhmg5sv")) as GameObject); //Firing;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/Barrels/_x51in5w8o")) as GameObject); //Barrel;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/LoadingMechanisms/_zzh1k1eiq")) as GameObject); //Loader;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/Triggers/_k5kxxqrjl")) as GameObject); //Trigger;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/Triggers/_k5kxxqrjl")) as GameObject); //Trigger;
        testObjects.Add(MonoBehaviour.Instantiate(Resources.Load<GameObject>("WeaponParts/FiringMechanisms/_shbhmg5sv")) as GameObject); //Firing;

        int index = 0;
        foreach (GameObject gg in testObjects)
        {
            if (index == 0)
            {
                index++;
                continue;
            }

            gg.transform.position = new Vector3(100, 100, 100);
            index++;
        }

        teardowngameobjects = testObjects;

        return testObjects.ToArray();

       
    }

    [UnityTest]
    public IEnumerator WeaponStructureAction_Replaces_TopLevel_Part_Correctly()
    {
        var parts = loadTestObjects();

        var structure = parts[0].GetComponent<WeaponStructure>();
        var go = parts[0];

        parts[1].transform.parent = go.transform;
        parts[1].transform.localPosition = Vector3.zero;

        structure.trigger = parts[1].GetComponent<WeaponPart>();

        yield return null;
        

        var replaceAction = new WeaponStructureAction(parts[1].GetComponent<WeaponPart>(), parts[6].GetComponent<WeaponPart>(), structure);
        replaceAction.BeginAction();

        while (!replaceAction.Finalized)
            yield return null;

        yield return null;

        Assert.IsTrue(parts[6].transform.IsChildOf(parts[0].transform));
        Assert.IsTrue(parts[1].gameObject == null);
        Assert.IsTrue(parts[6].GetComponent<WeaponPart>() == structure.trigger);

        var addAction = new WeaponStructureAction(parts[6].GetComponent<WeaponPart>().Adaptors[0], parts[2].GetComponent<WeaponPart>(), structure);
        addAction.BeginAction();
        while (!addAction.Finalized)
            yield return null;

        yield return new WaitForSeconds(2);

        var replaceAction2 = new WeaponStructureAction(structure.trigger, parts[7].GetComponent<WeaponPart>(), structure);
        replaceAction2.BeginAction();
        while (!replaceAction2.Finalized)
            yield return null;

        yield return new WaitForSeconds(2);

        Assert.IsTrue(parts[7].transform.IsChildOf(parts[0].transform));
        Assert.IsTrue(parts[6].gameObject == null);
        Assert.IsTrue(parts[2].transform.IsChildOf(parts[7].transform));
        Assert.IsTrue(parts[7].GetComponent<WeaponPart>() == structure.trigger);

    }

    [UnityTest]
    public IEnumerator WeaponStructureAction_Replaces_Parts_Correctly()
    {
        var parts = loadTestObjects();

        var structure = parts[0].GetComponent<WeaponStructure>();
        var go = parts[0];

        parts[1].transform.parent = go.transform;
        parts[1].transform.localPosition = Vector3.zero;

        structure.trigger = parts[1].GetComponent<WeaponPart>();

        yield return null;

        var action = new WeaponStructureAction(structure.trigger.Adaptors[0], parts[2].GetComponent<WeaponPart>(), structure);
        action.BeginAction();

        while (!action.Finalized)
            yield return null;



        var action2 = new WeaponStructureAction(parts[2].GetComponent<WeaponPart>().Adaptors[0], parts[3].GetComponent<WeaponPart>(), structure);
        action2.BeginAction();

        while (!action2.Finalized)
            yield return null;

        yield return new WaitForSeconds(2);

        var replacementAction = new WeaponStructureAction(parts[2].GetComponent<WeaponPart>(), parts[5].GetComponent<WeaponPart>(), structure);
        replacementAction.BeginAction();

        while (!replacementAction.Finalized)
            yield return null;

        yield return new WaitForSeconds(2);

        Assert.IsTrue(parts[2].gameObject == null);
        Assert.IsTrue(parts[5].transform.IsChildOf(parts[1].transform));

        var replacementAction2 = new WeaponStructureAction(parts[3].GetComponent<WeaponPart>(), parts[8].GetComponent<WeaponPart>(), structure);
        replacementAction2.BeginAction();

        while (!replacementAction2.Finalized)
            yield return null;

        yield return new WaitForSeconds(2);

        Assert.IsTrue(parts[3].gameObject == null);
        Assert.IsTrue(parts[8].transform.IsChildOf(parts[1].transform));


    }

    [UnityTest]
    public IEnumerator WeaponStructureAction_Adds_Parts_Correctly_And_Deletes_Them()
    {
       

        var parts = loadTestObjects();

        var structure = parts[0].GetComponent<WeaponStructure>();
        var go = parts[0];
       
        parts[1].transform.parent = go.transform;
        parts[1].transform.localPosition = Vector3.zero;

        structure.trigger = parts[1].GetComponent<WeaponPart>();

        yield return null;

        var action = new WeaponStructureAction(structure.trigger.Adaptors[0], parts[2].GetComponent<WeaponPart>(), structure);
        action.BeginAction();

        while (!action.Finalized)
            yield return null;

        

        var action2 = new WeaponStructureAction(parts[2].GetComponent<WeaponPart>().Adaptors[0], parts[3].GetComponent<WeaponPart>(), structure);
        action2.BeginAction();

        while (!action2.Finalized)
            yield return null;

        var action3 = new WeaponStructureAction(parts[3].GetComponent<WeaponPart>().Adaptors[0], parts[4].GetComponent<WeaponPart>(), structure);
        action3.BeginAction();

        while (!action3.Finalized)
            yield return null;

        Assert.IsTrue(parts[1].gameObject.transform.IsChildOf(parts[0].transform));
        Assert.IsTrue(parts[2].gameObject.transform.IsChildOf(parts[1].transform));
        Assert.IsTrue(parts[3].gameObject.transform.IsChildOf(parts[2].transform));
        Assert.IsTrue(parts[4].gameObject.transform.IsChildOf(parts[3].transform));



        yield return new WaitForSeconds(2);

        var removeAction = new WeaponStructureAction(parts[4].GetComponent<WeaponPart>(), structure);
        removeAction.BeginAction();

        while (!removeAction.Finalized)
            yield return null;

        yield return new WaitForSeconds(1);

        var removeAction1 = new WeaponStructureAction(parts[3].GetComponent<WeaponPart>(), structure);
        removeAction1.BeginAction();

        while (!removeAction1.Finalized)
            yield return null;

        yield return new WaitForSeconds(1);

        var removeAction2 = new WeaponStructureAction(parts[2].GetComponent<WeaponPart>(), structure);
        removeAction2.BeginAction();

        while (!removeAction2.Finalized)
            yield return null;

        yield return new WaitForSeconds(2);


        Assert.IsTrue(parts[2] == null);
        Assert.IsTrue(parts[3] == null);
        Assert.IsTrue(parts[4] == null);

    }


    [TearDown]
    public void AfterEveryTest()
    {
        foreach(GameObject go in teardowngameobjects)
        {
            if (go != null)
                GameObject.Destroy(go);
        }
    }
   
}
