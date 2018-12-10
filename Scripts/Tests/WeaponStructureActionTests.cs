using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using XOAProductions.WeaponDesigner;

public class WeaponStructureActionTests {

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


        return testObjects.ToArray();
    }


    [UnityTest]
    public IEnumerator WeaponStructureAction_Adds_Parts_Correctly()
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

        yield return new WaitForSeconds(10);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator WeaponStructureActionTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
}
