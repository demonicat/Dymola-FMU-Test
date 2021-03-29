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
    float left_stick_left, left_stick_right, left_stick_up, left_stick_down, right_stick_up, right_stick_down = 0;
    [SerializeField] float gain = (float)5;
    const int targetFPS = 300;

    void Awake()
    {
        Application.targetFrameRate = targetFPS;
        inputActions = new DroneController();
        OnEnable();
        ProcessInput();

        //Instantiate the FMU
        fmu = new FMU("Opt_Ideal1", name);
        fmu.Reset();
        fmu.SetupExperiment(Time.time);
        fmu.EnterInitializationMode();
        fmu.ExitInitializationMode();
        //float samplePeriod = 1 / (float) targetFPS;
        //fmu.SetReal("controlModule_Synchronous.samplePeriod", samplePeriod);

    }

    private void ProcessInput()
    {
        inputActions.Gameplay.X_Neg.performed += ctx => left_stick_left = ctx.ReadValue<float>();
        inputActions.Gameplay.X_Neg.canceled += ctx => left_stick_left = 0;
        inputActions.Gameplay.X_Pos.performed += ctx => left_stick_right = ctx.ReadValue<float>();
        inputActions.Gameplay.X_Pos.canceled += ctx => left_stick_right = 0;
        inputActions.Gameplay.Y_Neg.performed += ctx => left_stick_down = ctx.ReadValue<float>();
        inputActions.Gameplay.X_Neg.canceled += ctx => left_stick_down = 0;
        inputActions.Gameplay.Y_Pos.performed += ctx => left_stick_up = ctx.ReadValue<float>();
        inputActions.Gameplay.Y_Pos.canceled += ctx => left_stick_up = 0;
        inputActions.Gameplay.Z_Pos.performed += ctx => right_stick_up = ctx.ReadValue<float>();
        inputActions.Gameplay.Z_Pos.canceled += ctx => right_stick_up = 0;
        inputActions.Gameplay.Z_Neg.performed += ctx => right_stick_down = ctx.ReadValue<float>();
        inputActions.Gameplay.Z_Neg.canceled += ctx => right_stick_down = 0;
    }

    void FixedUpdate()
    {
        setCoord();
        print("X: " + xcoord + " Y: " +  ycoord + " Z: " + zcoord);
        //print(fmu.GetReal("controlModule_Synchronous.samplePeriod"));

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
        xcoord -= gain * Time.deltaTime * left_stick_left;
        xcoord += gain * Time.deltaTime * left_stick_right;
        ycoord -= gain * Time.deltaTime * left_stick_down;
        ycoord += gain * Time.deltaTime * left_stick_up;
        zcoord -= gain * Time.deltaTime * right_stick_down;
        zcoord += gain * Time.deltaTime * right_stick_up;
        fmu.SetReal("xcoord", xcoord);
        fmu.SetReal("zcoord", ycoord);
        fmu.SetReal("ycoord", zcoord);
    }


    //Controller enable
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
