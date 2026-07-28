using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using ScriptableObjects;
using Unity.Mathematics;
using Utility;


[AddComponentMenu("BobaTeaScripts/GameObjectTransformer")]
public class GameObjectTransformer : MonoBehaviour
{
    // Start is called before the first frame update
    // this will contain a bunch of Methods for what could happen to the Object. 
    // it will also contain a delegate that will run when a public method is called
    public enum TransformationTypes { Translation, Teleport, RotateAround,SnapRotateAround,Scale}  
    [SerializeField, Tooltip("the Transformation to perform on the Object")]
    private TransformationTypes transformationType;
    public UnityEvent onCompletion;
    [Tooltip("A Game Event to call on completion")]
    public GameEvent gameEvent;
    //private List<IEnumerator> _coroutines = new List<IEnumerator>(); // the private list of Actions to run. 
    
    
    //private IEnumerator _coroutine;
    public Transform target; // the transform this will target for its CoRoutines

    public float transformMoveSpeed;    //used for translations

    public float transformScaleSpeed;      //used for scales
    public float transformRotationSpeed;   // used for Rotations

    [Tooltip("rotation in angles for rotation transformations")]
    public float xRotation;
    public float yRotation;
    public float zRotation;

    public float xScale;
    public float yScale;
    public float zScale;
    // these will be based off of the Developers chosen transformations
    // we will make these coRoutines so they can be contained in a list I suppose :o

    /*public void DelTeleportTo(Transform target)
    {
        // this translates an objects position to a location
        // it will do a move to until its finished and then will return an accomplished task.
        transform.position = target.position;
    }*/

    /*public void DelTranslateTo(Transform target)
    {
        // this will shift a position over time then return true;
        transform.position = target.position;
    }*/
    
    public void RunTransformation()
    {
        IEnumerator coroutine;
        switch (transformationType)
        {
            case TransformationTypes.Translation:
                coroutine = TranslateTo();
                StartCoroutine(TransformationCoRoutineRunner(coroutine));
                break;
            case TransformationTypes.Teleport:
                coroutine = TeleportTo();
                StartCoroutine(TransformationCoRoutineRunner(coroutine));
                break;
            case TransformationTypes.RotateAround:
                coroutine = RotateAround();
                StartCoroutine(TransformationCoRoutineRunner(coroutine));
                break;
            case TransformationTypes.SnapRotateAround:
                coroutine = SnapRotateAround();
                StartCoroutine(TransformationCoRoutineRunner(coroutine));
                break;
            case TransformationTypes.Scale:
                coroutine = ScaleTo();
                StartCoroutine(TransformationCoRoutineRunner(coroutine));
                break;
        }
    }

    public void RunReverseTransformation()
    {
        // this will work like the regular coroutine except call a reversed transformation.
    }

    public void CancelTransformations()
    {
        
    }

    IEnumerator TeleportTo() // this will pass a coroutine to the owner to execute. that way all of the coroutines a skeleton is running is held by him
    {
        // this will translate the Object to a position
        transform.position = target.position;
        return null;
    }

    IEnumerator TranslateTo()
    { 
        // heres where we shift the transform each frame until we reach the target position
        Debug.Log(transformationType);
        while(Vector3.Distance(transform.position, target.position) >= 0.1f)
        {
            var step = transformMoveSpeed * Time.deltaTime;
            Debug.Log(step);
            transform.position = Vector3.MoveTowards(transform.position ,target.position, step);
            yield return null; // wait till the next frame
        }
    }

    IEnumerator RotateAround()      // this will rotate the object in world space around another object. yeehaw.
    {
        float xangleRotated = 0;
        float yangleRotated = 0;
        float zangleRotated = 0;
        while ((xangleRotated < Mathf.Abs(xRotation)) ||(yangleRotated < Mathf.Abs(yRotation)) || (zangleRotated < Mathf.Abs(zRotation)))
        {
            // this is the main loop we will write the x rotation first
            // we will grab the difference between the x angle goal and the current rotated
            // then clamp it to the rotate speed
            // which will set the distance it will rotate this loop.
            if (xRotation > 0)
            {
                float xstep = xRotation - xangleRotated;
                xstep = Mathf.Clamp(xstep, 0, (transformRotationSpeed * Time.deltaTime));
                transform.RotateAround(target.position, Vector3.right, xstep);
                xangleRotated += xstep;
            }
            else
            {
                float xstep = xRotation + xangleRotated;
                xstep = Mathf.Clamp(xstep, (-transformRotationSpeed * Time.deltaTime), 0);
                transform.RotateAround(target.position, Vector3.right, xstep);
                xangleRotated += Mathf.Abs(xstep); // we get the absolute value because it may be negative
            }
            if (yRotation > 0)
            {
                float ystep = yRotation - yangleRotated; 
                ystep = Mathf.Clamp(ystep,0, transformRotationSpeed);
                transform.RotateAround(target.position, Vector3.up, ystep);
                yangleRotated += ystep;
            }
            else
            {
                float  ystep = yRotation + yangleRotated;
                ystep = Mathf.Clamp(ystep, (-transformRotationSpeed * Time.deltaTime), 0);
                transform.RotateAround(target.position, Vector3.up, ystep);
                yangleRotated += Mathf.Abs(ystep);
            }

            if (zRotation > 0)
            {
                float zstep = zRotation - zangleRotated;
                zstep = Mathf.Clamp(zstep, 0, transformRotationSpeed);
                transform.RotateAround(target.position, Vector3.forward, zstep);
                zangleRotated += zstep;                
            }
            else
            {
                float  zstep = zRotation + zangleRotated;
                zstep = Mathf.Clamp(zstep, (-transformRotationSpeed * Time.deltaTime), 0);
                transform.RotateAround(target.position, Vector3.forward, zstep);
                zangleRotated += Mathf.Abs(zstep);
            }
            Debug.Log(zangleRotated+" "+xangleRotated+" "+yangleRotated);
            Debug.Log(zRotation + " "+xangleRotated+" "+yangleRotated);
            yield return null;
        }
    }

    IEnumerator SnapRotateAround() //this will rotate around another object in world space or something
    {
        // here is where we will to the snap rotations
        transform.Rotate(xRotation, yRotation, zRotation);
        return null;

    }

    IEnumerator ScaleTo()
    {
        // heres where we will scale the object to a new scale.
        float xScaled = 0;
        float yScaled = 0;
        float zScaled = 0;
        while ((xScaled >= xScale) || (yScaled >= yScale) || (zScaled >= zScale))
        {
            float xstep = xScaled - xScale;
            xstep = Mathf.Clamp(xstep, 0, transformScaleSpeed);
            transform.localScale = new Vector3(transform.localScale.x+xstep, transform.localScale.y, transform.localScale.z);
            xScaled += xstep;
            float ystep = yScaled - yScale;
            ystep = Mathf.Clamp(ystep, 0, transformScaleSpeed);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + ystep, transform.localScale.z);
            yScaled += ystep;
            float zstep = zScaled - zScale;
            zstep = Mathf.Clamp(zstep, 0, transformScaleSpeed);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + zstep);
            zScaled += zstep;
            yield return null;
        }
    }

    IEnumerator TransformationCoRoutineRunner(IEnumerator routine)
    {
        Debug.Log("coroutine RunnerStarted");
        // heres where we will run the selected coroutine and then at the end invoke our unity event
        yield return routine; //start the chosen coroutine and wait for it to end before the next step
        Debug.Log("CoroutineFinished");
        onCompletion.Invoke();
        if (gameEvent != null)
        {
            gameEvent.Raise(); // tell the game event to raise itself. calling all the listeners.
        }
    }
    
}
