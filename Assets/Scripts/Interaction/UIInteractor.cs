using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utility;

public class UIInteractor : MonoBehaviour
{
    PointerEventData _pointerEventData;
    EventSystem _eventSystem;
    private static UIInteractor _instance;

    private bool _isHolding = false; // should be used and checked to see if the cursor is dragging an object
    public bool IsHolding{get{return _isHolding;}}
    public FloatVariable interactRange;
    public float InteractRange{get{return interactRange.Value;}}

    public static UIInteractor Instance
    {
        get { return _instance; }
    }

    public void SetHoldingStatus(bool status)
    {
        _isHolding = status;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            Debug.Log("MouseInteractor instance set!");
        }

        _eventSystem = GetComponent<EventSystem>();
        //MouseFollower.inventoryUpdateEvent = inventoryUpdateEvent;
        //graphicRaycasterRuntimeSet.Initialize();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _pointerEventData = new PointerEventData(_eventSystem);
            _pointerEventData.position =
                Input.mousePosition; // grab the position of the mouse and shove it onto the PointerEventData
            List<GraphicRaycaster> graphicRaycasters = new List<GraphicRaycaster>();
            for (int i = 0; i < UISystem.UIReporterRuntimeSet.GetItems().Count; i++)
            {
                Debug.Log(UISystem.UIReporterRuntimeSet.GetItems().Count);
                graphicRaycasters.Add(UISystem.UIReporterRuntimeSet.GetItemIndex(i).GetComponent<GraphicRaycaster>());
            }

            List<RaycastResult>
                results =
                    new List<RaycastResult>(); // this needs to be a list that goes through every graphics raycaster in a List. that graphics raycaster list will be managed through a Scriptable object
            foreach (GraphicRaycaster graphicRaycaster in graphicRaycasters)
            {
                //Debug.Log(graphicRaycasters.Count);
                var newResults = new List<RaycastResult>();
                graphicRaycaster.Raycast(_pointerEventData, newResults);

                results.AddRange(newResults);
            }

            if (!UIInteractor.Instance.IsHolding)
            {
                foreach (RaycastResult result in results)
                {
                    InteractableComponent interactable =
                            result.gameObject
                                .GetComponent<
                                    InteractableComponent>(); // wrote this in here to avoid doing more than one Getcomponent per loop
                    if (interactable != null)
                    {
                        // InventoryManager.instance.CreateMovingItem(_slot.heldItem,_slot); // << we are moving this to be handled by the inventory slot.
                        interactable.Interact();
                    }
                }
            }
        }
    }
}

public class MouseFollower : MonoBehaviour // this is the class for making an item follow the mouse
{
    PointerEventData _pointerEventData;
    EventSystem _eventSystem;
    public GameObject _spriteHolder;
    private GameObject spriteCanvas;
    MeshRenderer _meshRenderer;
    Rigidbody _rbd;
    public static GameEvent InventoryUpdateEvent; //= UISystem.UpdateInventoryUI;
    public UIRuntimeSet UIReporterRuntimeSet;
    public ItemData movingItemData;
    private FloatVariable interactRange{get{return UIInteractor.Instance.interactRange;}} // writing this just so I dont have to rewrite more later
    protected GameObject _itemChild;
    protected GameObject _spriteChild;
    

    public ItemGridSpaceInteractable
        itemOrigin
    {
        private get;
        set;
    } // this is so it can be set by the inventory manager for in inventory transfers. but remains empty for items picked off the ground
    // may also be used for containers (chests and boxes in game I mean).

    void Awake()
    {
        Debug.Log("Mouser awakened");
        InventoryUpdateEvent = UISystem.UpdateInventoryUI;
        _rbd = GetComponent<Rigidbody>();
        _eventSystem = GetComponent<EventSystem>();
        _meshRenderer = GetComponent<MeshRenderer>();
        UIReporterRuntimeSet = UISystem.UIReporterRuntimeSet;
    }

    void OnEnable()
    {
        UIInteractor.Instance.SetHoldingStatus(true);
    }

    void OnDisable()
    {
        UIInteractor.Instance.SetHoldingStatus(false);
        Destroy(_spriteChild); //Consider Moving this to OnDisable. Its creation to On enable.
        Destroy(spriteCanvas);
    }

    void Update()
    {
        _pointerEventData = new PointerEventData(_eventSystem);
        _pointerEventData.position =
            Input.mousePosition; // grab the position of the mouse and shove it onto the PointerEventData
        List<RaycastResult> results = new List<RaycastResult>();
        List<GraphicRaycaster>
            graphicRaycasters =
                new List<GraphicRaycaster>(); //heres where we loop through all the raycast objects and add them to a list  
        for (int i = 0; i < UIReporterRuntimeSet.GetItems().Count; i++)
        {
            graphicRaycasters.Add(UIReporterRuntimeSet.GetItemIndex(i)
                .GetComponent<GraphicRaycaster>()); // this will grab all the graphicRaycasters.
        }

        foreach (GraphicRaycaster graphicRaycaster in graphicRaycasters)
        {
            var newResults = new List<RaycastResult>(); // creats a new list of raycasts results             
            graphicRaycaster.Raycast(_pointerEventData,
                newResults); //passes the pointer event data to the newresults list 
            results.AddRange(newResults); //adds newResults to the results list
        }

        if (results.Count != 0) //check if we are over a result (A ui element in the runtime set)
        {
            _spriteChild.SetActive(true); //set the spite to be on    
            if (_itemChild)
            {
                _itemChild.SetActive(false);
            } //set the meshRenderer to be off. this wont work with slime Objects
            
            _spriteChild.transform.position = Input.mousePosition; // tells the moving bject to be where the mouse is
        }
        else
        {
            _spriteChild.SetActive(false); // works for the sprite holder
            _itemChild.SetActive(true); //turns of the meshrender mesh
            transform.position = ItemCaster(); // tells the mesh to be at the ITemcast methods returned position (its a spot near the cursor)
        }


        if (Input.GetMouseButtonUp(0))
        {
            //create the world position here if the mouse button dropped it in the world. or into a slot (swapping if necessary). we will do a raycast looking for inventory spaces
            //
            foreach (RaycastResult result in results)
            {
                IReceiver _slot = result.gameObject.GetComponent<IReceiver>(); // check for a component that implements the Reciever interface.
                if (_slot != null ) // if there is indeed an interactable and it is implementing the reciever interface 
                {
                    ItemData itemToStore = movingItemData; //
                    //itemToStore.SetItemData(movingItemData);
                    if (_slot.Receive(itemToStore, itemOrigin))
                    {
                        MouserCatch(); //
                        Destroy(_itemChild);        //dESTROY tHE cHILD
                    }
                    else
                    {
                        //slot couldnt take item here. Drop the child
                        MouserCatch();
                    }
                    return;
                }
                else  // if there is no inventory slot then it goes back to its original slot
                {
                    if (itemOrigin.ItemGridSpace.OwningGrid != null) // we will check that the inventory it came from exists, if so it will attempt to perform an addto method, if it fails we will need to drop the item
                    {
                        if (!itemOrigin.ItemGridSpace.AddItem(movingItemData))   // TRY adding the Item to the grid. if it fails then do the drop
                        {
                            WorldItem.Drop(transform.position, Quaternion.identity, movingItemData);
                            Destroy(gameObject);
                            //this is where the code for dropping an item will go, as otherwise the item will just be destroyed
                        }

                        InventoryUpdateEvent.Raise();
                        MouserCatch();
                    }
                    else
                    {
                        //code needs to go here for dropping the item. since it cant find a slot to go back to it must instead Drop onto the ground as an item, preventing the itemdata being lost completely
                    }

                    return;
                }
                // if there is no UI at all then it is spawned into the game as an object
            }

            int layerObject = 6;
            Vector2 ray = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(ray, ray, layerObject);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<IReceiver>() != null)
                {
                    InteractableComponent item = hit.collider.gameObject.GetComponent<InteractableComponent>();
                    if (item.isInteractable == true)
                    {
                        //Debug.Log("is interactable!");        // I believe this is old code for the 2d implementation of this script
                        
                        //Destroy(hit.collider.gameObject);       // should probably add something to delete its material too since apparently that is left behind as its own thing
                    }
                }
            }

            MouserCatch(); //destroy the MouseFollowerGameObject.

        }
    }
    public Vector3 ItemCaster()                         // this method will shoot a line out of the camera towards the mouse for item dropping and moving
    {
        Vector3 basePos = Camera.main.transform.position; // set the Position that we will shoot the base of the Raycast from
        Vector3 newpos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, interactRange.Value);
        newpos = Camera.main.ScreenToWorldPoint(newpos); // first we grab the position of the mouse in screenspace
        Vector3 newdir = newpos - basePos; // then we get the direction between the camera object and the mouse
        // this might not work if the mousinteractor is not on the main viewing camera
        newdir = Vector3.Normalize(newdir);
        newpos = basePos + (newdir * interactRange.Value);
        RaycastHit hit;
        if (Physics.Raycast(UIInteractor.Instance.transform.position, newdir,out hit, interactRange.Value))
        {
            newpos = hit.point;
        }
        return newpos;
    }
    public void MouserCatch()           
    {                                                                           // this tells the mouser to turn collisions on for the item and then to destroy itself and the ui sprite
        /*Collider[] colls = gameObject.GetComponents<Collider>();
        foreach(Collider col in colls)
        {
            col.enabled = true;
        } */
        //_rbd.useGravity = true;
        if (_itemChild)
        {
            _itemChild.transform.SetParent(null);
            (bool componentCheck, Rigidbody rbd) rigidbodyCheck =
                ToskaUtilities.TestReturnComponent<Rigidbody>(_itemChild);
            if(rigidbodyCheck.componentCheck)
                rigidbodyCheck.rbd.useGravity = true;
            Collider[] colls = _itemChild.GetComponents<Collider>();
            foreach (Collider col in colls) // mayeb add a istrigger check here if we want the item to collide with things
            {
                col.enabled = true;        // woops! this will should make the object solid
            }
        }
        
        Destroy(gameObject);                 
        
    }
    public void MouserChase()
    {
        _rbd.useGravity = false;        // sets the rigidbody to not use gravity. this is so it doesnt build up speed while being held in place
        //MouseInteractor.instance.isHolding = true;  // we set this here but Im not sure it ever gets used yet? must be so that the mouse can be told not interact with other stuff
        Image image = _spriteHolder.AddComponent<Image>();
        //_spriteHolder.transform.SetParent(InventoryManager.instance.inventoryScreen.transform);// this needs to be setting the parents transform to that of a canvas maybe we can just find a component of type? Or reaching into our GraphicRaycaster Runtime
        _spriteHolder.transform.SetParent(spriteCanvas.transform);
        image.raycastTarget = false;
        image.sprite = gameObject.GetComponent<InventoryItem>().itemData.Sprite;
        Collider[] colls = gameObject.GetComponents<Collider>();
        foreach(Collider col in colls)          // mayeb add a istrigger check here if we want the item to collide with things
        {
            col.enabled = false;
        }
    }

    public void CreateSpriteChild(ItemData itemData)
    {   
        spriteCanvas = new GameObject("spriteCanvas",typeof(Canvas)); // here we create a new Gameobject with a canvas that will become the parent of the sprite holder
        spriteCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        spriteCanvas.GetComponent<Canvas>().sortingOrder = 1000;        //sets the canvas really high on the sorting order so it renders over everything else
        _spriteHolder = new GameObject("spriteHolder");
        Image image = _spriteHolder.AddComponent<Image>();
        _spriteHolder.AddComponent<CanvasRenderer>();
        _spriteHolder.transform.SetParent(spriteCanvas.transform);
        image.raycastTarget = false;
        image.sprite = itemData.Sprite;
        _spriteChild = _spriteHolder;
    }

    public void CreateItemChild(ItemData itemData)
    {   
       // Debug.Log("CreateItemChild was called but is not implemented!!!");
            
        Debug.Log("CreateItemChild");
        _itemChild = Instantiate(itemData.WorldItemPrefab,gameObject.transform); // we need this to be made as a child of this object at its position..... ;
        _itemChild.transform.position = gameObject.transform.position; //we set its position to that of the mouseInteractor
        (bool componentTrue, Rigidbody rbd) rigidBodyTry =  ToskaUtilities.TestReturnComponent<Rigidbody>(_itemChild);
        if(rigidBodyTry.componentTrue)
            rigidBodyTry.rbd.useGravity = false;

        Collider[] colls = _itemChild.GetComponents<Collider>();
        
        foreach (Collider col in colls) // mayeb add a istrigger check here if we want the item to collide with things
        {
            col.enabled = false;
        }
        // might need to turn off all the colliders and turn them back on. ( disabling the raycast would be better but Icbf)
        
        // will need to generate the world Item of the ItemData here
        /*if (itemData is BobaItemData)
        {
            //here we will instead instantiate the boba Item. much like the boba drop script
            GameObject defaultItem = Resources.Load("DefaultBobaItem") as GameObject; 
            _itemChild = Instantiate(defaultItem, gameObject.transform);
            Item _item =_itemChild.GetComponent<Item>();
            _item.interactRangeVar = interactRange;
            _item.itemData = itemData;
        }
        else if (itemData is CoinData)
        {
            // heres where we handle creating an Item that is a coindata
            GameObject defaultItem = Resources.Load("Coin") as GameObject;
            _itemChild = Instantiate(defaultItem, gameObject.transform);
            Item _item = _itemChild.GetComponent<Item>();
            _item.interactRangeVar = interactRange;
            _item.itemData = itemData;
        }
        else
    
            Debug.Log("CreateItemChild");
            _itemChild =
                new GameObject("itemChild"); // we need this to be made as a child of this object at its position..... ;

            _itemChild.transform.position =
                gameObject.transform.position; //we set its position to that of the mouseInteractor
            _itemChild.transform.SetParent(gameObject.transform);
            Item _item = _itemChild.AddComponent<Item>();
            _item.interactRangeVar = interactRange;
            _item.BuildFromItemData(itemData);  
        }
        _itemChild.GetComponent<Rigidbody>().useGravity = false;

        Collider[] colls = _itemChild.GetComponents<Collider>();
        
        foreach (Collider col in colls) // mayeb add a istrigger check here if we want the item to collide with things
        {
            col.enabled = false;
        }
        // might need to turn off all the colliders and turn them back on. ( disabling the raycast would be better but Icbf)
        
        */
    }
}
