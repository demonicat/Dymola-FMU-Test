using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMI2;

//Basic test of FMU input and output visualized with Unity.
public class DroneMovement : MonoBehaviour
{
    FMU fmu;
    [SerializeField]float[] coord = new float[] { 0, 0, 0 };

    void Start()
    {
        //Instantiate the FMU
        fmu = new FMU("DroneSimulation_IdealMachine", name);
        fmu.Reset();
        fmu.SetupExperiment(Time.time);
        //Set target goal
        setCoord(coord[0], coord[1], coord[2]);
        fmu.EnterInitializationMode();
        fmu.ExitInitializationMode();

    }

    void FixedUpdate()
    {
        //Synchronize the model with the current time
        fmu.DoStep(Time.time, Time.deltaTime);
        //Read FMU outputs of position 
        float xdir = (float)fmu.GetReal("xgps");
        float ydir = (float)fmu.GetReal("ygps");
        float zdir = (float)fmu.GetReal("zgps");
        //Assign gps values to transform of object
        transform.position = new Vector3(xdir, ydir, zdir);
    }

    //Function to pass through ideal coordinates
    void setCoord(float a, float b, float c)
    {
        coord[0] = a;
        coord[1] = b;
        coord[2] = c;
        fmu.SetReal("xcoord", coord[0]);
        fmu.SetReal("zcoord", coord[1]);
        fmu.SetReal("ycoord", coord[2]);
    }

    //General clean up
    void OnDestroy()
    {
        fmu.FreeInstance();
        fmu.Dispose();
    }

}
