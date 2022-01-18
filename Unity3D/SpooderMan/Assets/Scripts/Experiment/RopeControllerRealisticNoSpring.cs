using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// simulate a rope with verlet integration and no springs
public class RopeControllerRealisticNoSpring : MonoBehaviour
{
    [SerializeField] private int numberOfRopeSections = 15;
    [SerializeField] private float ropeWidth = 0.2f;

    // objects that will interact with the rope
    public Transform whatTheRopeIsConnectedTo;
    public Transform whatIsHangingFromTheRope;

    // line renderer used to display the rope
    private LineRenderer lineRenderer;

    // a list with all rope sections
    private List<RopeSection> allRopeSections = new List<RopeSection>();

    // rope data
    private float ropeSectionLength;

    private void Start()
    {
        // init the line renderer
        lineRenderer = GetComponent<LineRenderer>();

        // create the rope
        Vector3 ropeSectionPos = whatTheRopeIsConnectedTo.position;
        Vector3 deltaPos = ropeSectionPos - whatIsHangingFromTheRope.position;
        Vector3 section = deltaPos / numberOfRopeSections;
        ropeSectionLength = section.magnitude;

        for (int i = 0; i < numberOfRopeSections; ++i)
        {
            allRopeSections.Add(new RopeSection(ropeSectionPos));
            ropeSectionPos -= section;
        }
    }

    private void Update()
    {
        // display the rope with the line renderer
        DisplayRope();

        // move what is hanging from the rope to the end of the rope
        whatIsHangingFromTheRope.position = allRopeSections[allRopeSections.Count - 1].pos;

        // make what's hanging from the rope look at the second to last rope position to 
        // make it rotate with the rope (so essentially the object will be rotated towards
        // the pivot point and look up to it)
        whatIsHangingFromTheRope.LookAt(allRopeSections[allRopeSections.Count - 2].pos);
    }

    private void FixedUpdate()
    {
        UpdateRopeSimulation();
    }

    private void UpdateRopeSimulation()
    {
        Vector3 gravityVec = new Vector3(0f, -9.81f, 0f);

        float time = Time.fixedDeltaTime;

        // move the first section to what the rope is connected to
        RopeSection firstRopeSection = allRopeSections[0];

        firstRopeSection.pos = whatTheRopeIsConnectedTo.position;

        allRopeSections[0] = firstRopeSection;

        // move the other rope sections with Verlet integration
        for (int i = 1; i < allRopeSections.Count; ++i)
        {
            RopeSection currentRopeSection = allRopeSections[i];

            // calculate velocity this update
            Vector3 vel = currentRopeSection.pos - currentRopeSection.oldPos;

            // update the old position with the current position
            currentRopeSection.oldPos = currentRopeSection.pos;

            // find the new position
            currentRopeSection.pos += vel;

            currentRopeSection.pos += gravityVec * time;

            // add it back to the array
            allRopeSections[i] = currentRopeSection;
        }

        // make sure the rope sections have the correct lengths
        for (int i = 0; i < numberOfRopeSections + 5; ++i)
        {
            ImplementMaximumStretch();
        }
        
    }

    // TODO problem is here!!! (cube swings less over time)
    private void ImplementMaximumStretch()
    {
        for (int i = 0; i < allRopeSections.Count - 1; ++i)
        {
            RopeSection topSection = allRopeSections[i];

            RopeSection bottomSection = allRopeSections[i + 1];

            // the distance between the sections
            float dist = (topSection.pos - bottomSection.pos).magnitude;

            // what's the stretch/compression
            float distError = Mathf.Abs(dist - ropeSectionLength);

            Vector3 changeDir = Vector3.zero;

            // compress this section
            if (dist > ropeSectionLength)
            {
                changeDir = (topSection.pos - bottomSection.pos).normalized;
            }
            // extend this section
            else if (dist < ropeSectionLength)
            {
                changeDir = (bottomSection.pos - topSection.pos).normalized;
            }
            // do nothing
            else
            {
                continue;
            }

            Vector3 change = changeDir * distError;

            // TODO WE CHANGING POSITION HERE SO MAYBE THIS IS WHERE WE CAN TWEAK
            if (i != 0)
            {
                bottomSection.pos += change * 0.5f;
                allRopeSections[i + 1] = bottomSection;
                topSection.pos -= change * 0.5f;
                allRopeSections[i] = topSection;
            }
            // because the section is connected to something
            else
            {
                bottomSection.pos += change;
                allRopeSections[i + 1] = bottomSection;
            }
        }
    }

    // display the rope with a line renderer
    private void DisplayRope()
    {
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;

        // an array with all rope section positions
        Vector3[] positions = new Vector3[allRopeSections.Count];

        for (int i = 0; i < allRopeSections.Count; ++i)
        {
            positions[i] = allRopeSections[i].pos;
        }

        lineRenderer.positionCount = positions.Length;

        lineRenderer.SetPositions(positions);
    }

    // a struct that will hold information about each rope section
    public struct RopeSection
    {
        public Vector3 pos;
        public Vector3 oldPos;

        // to write RopeSection.zero
        public static readonly RopeSection zero = new RopeSection(Vector3.zero);

        public RopeSection(Vector3 pos)
        {
            this.pos = pos;
            this.oldPos = pos;
        }
    }
}
