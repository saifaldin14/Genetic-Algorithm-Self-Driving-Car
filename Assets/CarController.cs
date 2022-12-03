using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(NNet))]
public class CarController : MonoBehaviour
{
    private Vector3 startPosition, startRotation;
    private NNet network;

    [Range(-1f,1f)]
    public float a,t; // Acceleration and turning

    public float timeSinceStart = 0f; // Used to check if the car has been idle for too long

    [Header("Fitness")]
    public float overallFitness;
    // We value how far the car goes versus how fast it goes
    // These are the traits that are values through the generations
    public float distanceMultipler = 1.4f; // How important the distance is to the overall fitness
    public float avgSpeedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    [Header("Network Options")]
    public int LAYERS = 1;
    public int NEURONS = 10;

    private Vector3 lastPosition;
    private float totalDistanceTravelled;
    private float avgSpeed;

    private float aSensor,bSensor,cSensor;

    private void Awake() {
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<NNet>();
    }

    public void ResetWithNetwork (NNet net) {
        network = net;
        Reset();
    }

    public void Reset() {
        timeSinceStart = 0f;
        totalDistanceTravelled = 0f;
        avgSpeed = 0f;
        lastPosition = startPosition;
        overallFitness = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    // When the car hits the wall, reset
    private void OnCollisionEnter (Collision collision) {
        Death();
    }

    // Provides a constant and stable environment for the agent to be trained
    private void FixedUpdate() {

        InputSensors();
        lastPosition = transform.position;


        (a, t) = network.RunNetwork(aSensor, bSensor, cSensor);


        MoveCar(a,t);

        timeSinceStart += Time.deltaTime;

        CalculateFitness();
    }

    private void Death () {
        GameObject.FindObjectOfType<GeneticManager>().Death(overallFitness, network);
    }

    // Define custom OFs
    private float deJong(float[] x) {
        float v = 0f;

        foreach (float xi in x) {
            v += (float)Math.Pow(xi, 2);
        }

        return v;
    }

    private double rosenbrock(double a, double b, double x, double y) {
        return (Math.Pow(a - x, 2) + b * Math.Pow(y - Math.Pow(x, 2), 2));
    }

    private double himmelblau (double x, double y) {
        return (Math.Pow(Math.Pow(x, 2) + y - 11, 2) + Math.Pow(x + Math.Pow(y, 2) - 7, 2));
    }

    private void CalculateFitness() {
        totalDistanceTravelled += Vector3.Distance(transform.position,lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;

        float x = totalDistanceTravelled * distanceMultipler;
        float y = avgSpeed * avgSpeedMultiplier;
        float z = ((aSensor+bSensor+cSensor) / 3) * sensorMultiplier;

        // Test out De Jong OF (swap with the other 3)
        float[] deJongArray = {x, y, z};
        float deJongObjectiveFunction = deJong(deJongArray);
        overallFitness = x + y + z;

        // Car is too idle
        if (timeSinceStart > 20 && overallFitness < 40) {
            Death();
        }

        // The car has done too many laps
        if (overallFitness >= 1000) {
            Death();
        }
    }

    private void InputSensors() {

        Vector3 a = (transform.forward + transform.right); // Diagonal right
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward - transform.right); // Diagonal left

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit)) {
            aSensor = hit.distance / 20; // Ensures that the input is normalized (values between 0 - 1)

            // Used in testing to visualize the sensor lines
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = b;

        if (Physics.Raycast(r, out hit)) {
            bSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = c;

        if (Physics.Raycast(r, out hit)) {
            cSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

    }

    private Vector3 inp;
    public void MoveCar (float v, float h) {
        float verticalMoveConstant = 11.4f;
        int horizontalMoveConstant = 90;

        inp = Vector3.Lerp(Vector3.zero, new Vector3(0,0,v * verticalMoveConstant), 0.02f);
        inp = transform.TransformDirection(inp);
        transform.position += inp;

        transform.eulerAngles += new Vector3(0, (h * horizontalMoveConstant) * 0.02f,0);
    }

}
