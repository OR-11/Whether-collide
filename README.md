# Whether-collide
 This C# program is a collide progeam.

## How to use

1.Add this cs-file to Assets folder.  
2.Attach BoxCollider2D to the all of gameobjects you want to collide.  
3.Attach this cs-file to the gameobjects you just attached the collider.  
4.Attach each of gameobjects you want to collide to each of arrays named "grounds".

:heavy_exclamation_mark:BoxCollider2D's center must be set 0.  
You can use BoxCollider2D only.
It doesn't use Rigidbody2D.

##  Functions
###  ・movement
    Set motion.
  
###  ・change_dummy_transform_position
    You shouldn't use transform.position when you want the gameobject to change position.
    You can use this to change the object's position.
    
###  ・is_touching
    It returns whether touching the gameobjects that set arguments or not.
    
###  ・touching_Object
    It returns the gameobject of the number you set.
    
###  ・searching_object_withTag
    It returns whether touching the gameobject tagged in the arguments or not.
