using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMI2;
using UnityEngine.InputSystem; 

//Basic test of FMU input and output visualized with Unity.
public class DroneMovement : MonoBehaviour
{
    DroneController inputActions;
    FMU fmu;
    float xcoord, ycoord, zcoord = 0; 
    float xdir, ydir, zdir; 

    void Awake()
    {
        Application.targetFrameRate = 300; 

        inputActions = new DroneController();
        OnEnable();
        inputActions.Gameplay.X_Neg.performed += ctx => decreaseX();
        inputActions.Gameplay.X_Pos.performed += ctx => increaseX();
        inputActions.Gameplay.Y_Neg.performed += ctx => decreaseY();
        inputActions.Gameplay.Y_Pos.performed += ctx => increaseY();

        //Instantiate the FMU
        fmu = new FMU("DroneSimulation_IdealMachine", name);
        fmu.Reset();
        fmu.SetupExperiment(Time.time);
        fmu.EnterInitializationMode();
        fmu.ExitInitializationMode();

    }

    void FixedUpdate()
    {
        print("X: " + xcoord + " Y: " +  ycoord + " Z: " + zcoord);
        setCoord(); 

        //Synchronize the model with the current time
        fmu.DoStep(Time.time, Time.deltaTime);
        //Read FMU outputs of position 
        xdir = (float)fmu.GetReal("xgps");
        ydir = (float)fmu.GetReal("ygps");
        zdir = (float)fmu.GetReal("zgps");
        //Assign gps values to transform of object
        transform.position = new Vector3(xdir, ydir, zdir);
    }

    //Function to pass through ideal coordinates
    void setCoord()
    { 
        fmu.SetReal("xcoord", xcoord);
        fmu.SetReal("zcoord", ycoord);
        fmu.SetReal("ycoord", zcoord);
    }

    void decreaseX()
    {
        xcoord -= (float)0.01;
    }

    void increaseX()
    {
        xcoord += (float)0.01;
    }

    void decreaseY()
    {
        ycoord -= (float)0.01;
    }

    void increaseY()
    {
        ycoord += (float)0.01;
    }

    void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }
    //General clean up
    void OnDestroy()
    {
        fmu.FreeInstance();
        fmu.Dispose();
    }

}
