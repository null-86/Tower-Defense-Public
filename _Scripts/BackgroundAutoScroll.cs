using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAutoScroll : MonoBehaviour
{
    public Vector2 scrollVector = new Vector2(-1, 0);
    public SpriteRenderer backgroundImg;
    public LinkedList<SpriteRenderer> backgroundImgs;

    public int minImageCount = 2;
    public float deleteBackgroundBuffer = 0;
    
    private Vector2 cameraC;
    private Vector2 cameraBL;
    private Vector2 cameraTR;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImgs = new LinkedList<SpriteRenderer>();

        // check background; destroy self if null
        if (backgroundImg == null) {
            Debug.LogWarning("No background image; destroying controller.");
            Destroy(gameObject);
        }

        // calculate camera world space bounds
        CalculateCameraBounds();

        // disable children if exist (disable the background used for editor viewing)
        if (gameObject.transform.GetChild(0) != null) {
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }

        // create copies
        for(int i = 0; i < minImageCount; i++) {
            ExtendBackground();
        }
        
        

    }

    // Update is called once per frame
    void Update()
    {
        if (backgroundImgs.Count < 1) { return; }

        // move background
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).position = new Vector3(
                transform.GetChild(i).position.x + scrollVector.x * Time.deltaTime,
                transform.GetChild(i).position.y + scrollVector.y * Time.deltaTime,
                0
                );
        }
        
        // calculate the point where x is no longer on screen/camera in world pos
        float targetX = //transform.TransformPoint(
            new Vector2(
            backgroundImgs.First.Value.bounds.extents.x +
            backgroundImgs.First.Value.transform.position.x,
            backgroundImgs.First.Value.transform.position.y
            ).x;

        // if right border of image is off screen, move it to right and put on end of list
        if (targetX < cameraBL.x + deleteBackgroundBuffer) {
            RecycleBackgroundImage();
        }
    }



    void CalculateCameraBounds() {
        // todo - may cause issues if camera size changes mid-game. get callback from camera change event to fix'
        cameraC = Camera.main.ViewportToWorldPoint(new Vector2(.5f, .5f));
        cameraBL = Camera.main.ViewportToWorldPoint(Vector2.zero);
        cameraTR = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
    }

    void ExtendBackground() {

        // set next pivot point (x) - y is always same
        float x = (backgroundImgs.Count > 0) ?
            // add half width of current and half width of next to the current pos
            backgroundImgs.Last.Value.bounds.extents.x +
            backgroundImg.bounds.extents.x + 
            backgroundImgs.Last.Value.transform.position.x 
            :
            cameraBL.x + backgroundImg.bounds.extents.x;
        
        var img = Instantiate(
            backgroundImg,
            new Vector3(x, cameraC.y),
            Quaternion.identity,
            transform
        );

        backgroundImgs.AddLast(img);
    }
    
    private void RecycleBackgroundImage() {
        var img = backgroundImgs.First.Value;
        backgroundImgs.RemoveFirst();

        float x = (backgroundImgs.Count > 0) ?
            backgroundImgs.Last.Value.bounds.size.x +
            backgroundImgs.Last.Value.transform.position.x
            :
            cameraBL.x + backgroundImg.bounds.extents.x;

        img.transform.position = new Vector2(
                x, cameraC.y
            );
        backgroundImgs.AddLast(img);
    }
}
