using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The platform the weapon model floats above. 
/// Handles the floating animation and player input for rotation of the model.
/// </summary>
public class WeaponTurntable : MonoBehaviour {

    //strength and frequency of the floating animation
    public float weaponFloatAmplitude, weaponFloatFrequency;
    //starting-vector for floating
    public Vector3 weaponFloatBaseVector;
    //pauses floating
    public bool floatingPaused = false;

    //maps input to weapon model rotation
    public float rotationSensitivity = 900;

    //routine that handles mouse input
    private Coroutine mouseInputRoutine;
    //routine that handles float animation
    private Coroutine floatRoutine;
    //the point to rotate the model around
    public Vector3 rotationPoint;

	

	void Start () {
        floatRoutine = StartCoroutine(floatWeapon());
    }

    private void OnDrawGizmosSelected()
    {
        //display a sphere on the rotationPoint
        Gizmos.DrawSphere(transform.TransformPoint(rotationPoint), 0.2f);
    }

    
    void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            mouseInputRoutine = StartCoroutine(mouseInput());
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopCoroutine(mouseInputRoutine);
            floatingPaused = false;
        }

    }

    /// <summary>
    /// Handles rotating the weapon model by dragging with the mouse
    /// </summary>
    /// <returns></returns>
    private IEnumerator mouseInput()
    {
        var startingVector = Input.mousePosition; //starting mouseposition
        floatingPaused = true; //pause floating while rotating the model
        yield return new WaitForFixedUpdate();

        //takes the movement in mouse-x axis and transforms it into an angle by which the transform is rotated around vector3.up
        while (true)
        {
            float xOffset =  Input.mousePosition.x - startingVector.x;
            float angle = xOffset / rotationSensitivity * -90;
            this.transform.RotateAround(transform.TransformPoint(rotationPoint), Vector3.up, angle);
            startingVector = Input.mousePosition;
            weaponFloatBaseVector = new Vector3(transform.position.x, weaponFloatBaseVector.y, transform.position.z);
            yield return new WaitForFixedUpdate();
        }

    }

    /// <summary>
    /// Performs a sinus based up-down floating animation of the weapon model
    /// Currently used instead of the animator to make development quicker
    /// TODO: Replace this and use the animator instead
    /// </summary>
    /// <returns></returns>
    private IEnumerator floatWeapon()
    {
        //rotation in radians/pi
        float currentPos = 0;
        while (true)
        {
            if (floatingPaused)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            //last frametime times frequency adde
            currentPos += Time.deltaTime * weaponFloatFrequency;

            if (currentPos >= 2)
                currentPos = 0;//reset if >2pi

            float radians = Mathf.PI * currentPos; //convert to radians
            float sin = Mathf.Sin(radians); 

            float newYAxisValue = weaponFloatBaseVector.y + sin * weaponFloatAmplitude; //new y height is old height plus sinus*amplitude
            Vector3 newPos = new Vector3(weaponFloatBaseVector.x, newYAxisValue, weaponFloatBaseVector.z); 

            this.transform.position = newPos; //new position added to transform

            yield return new WaitForFixedUpdate();
        }
    }
}
