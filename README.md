# Whether-collide
 This C# program is a collide program.   
 I'm making it not to let players go through walls.

## How to use

1.Add this cs-file to Assets folder.  
2.Attach BoxCollider2D to the all of gameobjects you want to collide.  
3.Attach this cs-file to the gameobjects you just attached the collider.  
4.Attach each of gameobjects you want to collide to each of arrays named "grounds".

:heavy_exclamation_mark:BoxCollider2D's center must be set 0.  
You can use BoxCollider2D only.
It doesn't use Rigidbody2D.

##  Functions
###  ・Main()
    Main function. Use it when Function Settings' "Use function" is "None".

###  ・Addmovement(Vector2 MovementValue)
    Add movement's value. It's like Rigidbody's AddForce.

###  ・movement(Vector2 movementvalueforset)
    Set motion.
    
###  ・change_dummy_transform_position(bool absolute_x, float x, bool absolute_y, float y, [bool absolute_z = false, float z = 0, bool movinig_without_col = false])
    You shouldn't use transform.position when you want the gameobject to change position.
    You can use this to change the object's position.
   
###  ・is_touching(GameObject something)
    It'll returns whether colliding with "something".
   
###  ・touching_Object(int num)
    It'll return the gameobject of the number you set.
    
###  ・searching_object_withTag(string tag)
    It'll return whether touching the gameobject tagged in the arguments or not.
