using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class motion : MonoBehaviour
{
    public bool debuging;

    public bool only_whether_touch;

    public bool ground;

    public bool stop_when_this_collide;

    public float Air_resistance;
    public float gravity;

    public Vector2 scriptcol_x;
    public Vector2 scriptcol_y;

    public Vector2 befor_scriptcol_x;
    public Vector2 befor_scriptcol_y;

    public Vector2 movementvalue;

    Vector2 mysize;

    public Vector3 dummy_transform_position;
    public Vector3 befor_transform_position;

    public Vector3 dummy_transform_position_to_set;
    public bool set_befor_col;
    public bool do_dummy_transform_position_to_set_execute;

    BoxCollider2D box2d;

    motion script_motion;

    public GameObject[] grounds = new GameObject[0];

    public bool[] square_ground_wall_right;

    public bool[] square_ground_wall_left;

    public bool[] square_ground_wall_up;

    public bool[] square_ground_wall_down;

    //----------------------------------------------------------------------
    //座標の差(右/左:X座標 | 上/下:Y座標)の絶対値を格納する予定
    public float[] square_ground_wall_right_distance;

    public float[] square_ground_wall_left_distance;

    public float[] square_ground_wall_up_distance;

    public float[] square_ground_wall_down_distance;
    //Mathf.Abs(a) aが絶対値になって戻ってくる
    //GameObject
    //public GameObject[] square_ground_wall_right_distance_object;

    //public GameObject[] square_ground_wall_left_distance_object;

    //public GameObject[] square_ground_wall_up_distance_object;

    //public GameObject[] square_ground_wall_down_distance_object;
    //----------------------------------------------------------------------

    public bool touching_something;

    public bool touch_right;

    public bool touch_left;

    public bool touch_up;

    public bool touch_down;

    public GameObject[] touching_right;

    public GameObject[] touching_left;

    public GameObject[] touching_up;

    public GameObject[] touching_down;

    public GameObject[] touching;

    //自作コリダーたち
    //変数準備
    int count;// = grounds.Length;
    int check = 0;
    bool is_y_0 = false;
    bool is_x_0 = false;

    //a1：右上の点の比例定数
    //a2：左上の点の比例定数
    //a3：右下の点の比例定数
    //a4：左下の点の比例定数

    float a1 = 0;
    float a2 = 0;
    float a3 = 0;
    float a4 = 0;

    //b1：右上の点のb
    //b2：左上の点のb
    //b3：右下の点のb
    //b3：左下の点のb

    float b1 = 0;
    float b2 = 0;
    float b3 = 0;
    float b4 = 0;

    // Start is called before the first frame update
    void Start()
    {
        dummy_transform_position = transform.position;
        befor_transform_position = dummy_transform_position;

        colcount();

        box2d = GetComponent<BoxCollider2D>();
        script_motion = GetComponent<motion>();
        if (box2d == null || box2d.offset != new Vector2(0, 0))
        {
            Debug.LogError("Error:There isn't BoxCollider2D or the BoxCollider2D's offset isn't set 0");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debuging == true)
        {
            //scriptcol_x = new Vector2((dummy_transform_position.x + (box2d.size.x * (transform.localScale.x * 0.5f))), (dummy_transform_position.x + (-box2d.size.x * (transform.localScale.x * 0.5f))));
            //scriptcol_y = new Vector2((dummy_transform_position.y + (box2d.size.y * (transform.localScale.y * 0.5f))), (dummy_transform_position.y + (-box2d.size.y * (transform.localScale.y * 0.5f))));

            //Debug.Log("x:" + scriptcol_x + "y:" + scriptcol_y);

            drowline(new Vector3(scriptcol_x.x, scriptcol_y.x, -1), new Vector3(scriptcol_x.y, scriptcol_y.x, -1));

            drowline(dummy_transform_position, befor_transform_position);
            drowline(new Vector3(scriptcol_x.x, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.x, 0));
            drowline(new Vector3(scriptcol_x.y, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.x, 0));
            drowline(new Vector3(scriptcol_x.x, befor_scriptcol_y.y, 0), new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.y, 0));
            drowline(new Vector3(scriptcol_x.y, befor_scriptcol_y.y, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.y, 0));

            //if (Input.GetKey(KeyCode.Space)) movement(new Vector2(-1, 0), true);
            //else movement(new Vector2(0, 0), false);
            if (Input.GetKey(KeyCode.P)) Debug.Log(scriptcol_x);
            //if (Input.GetKey(KeyCode.D)) Debug.Log(Mathf.Approximately())
        }
    }

    private void FixedUpdate()
    {
        dummy_transform_position = transform.position;

        if (dummy_transform_position != befor_transform_position) Debug.Log("not same");

        scriptcol_x = new Vector2((dummy_transform_position.x + (box2d.size.x * (transform.localScale.x * 0.5f))), (dummy_transform_position.x + (-box2d.size.x * (transform.localScale.x * 0.5f))));
        scriptcol_y = new Vector2((dummy_transform_position.y + (box2d.size.y * (transform.localScale.y * 0.5f))), (dummy_transform_position.y + (-box2d.size.y * (transform.localScale.y * 0.5f))));
        mysize = transform.localScale;

        if (Input.GetKey(KeyCode.Space) && debuging) movement(new Vector2(-10, 0), true);
        else movement(new Vector2(0, 0), false);


        //Array.Fill(square_ground_wall_down_distance, -1);//←なんか出来ないからforで
        for (int count_ = square_ground_wall_down_distance.Length; count_ > 0; --count_)
        {
            square_ground_wall_down_distance[count_ - 1] = Mathf.Infinity;
        }
        for (int count_ = square_ground_wall_up_distance.Length; count_ > 0; --count_)
        {
            square_ground_wall_up_distance[count_ - 1] = Mathf.Infinity;
        }
        for (int count_ = square_ground_wall_left_distance.Length; count_ > 0; --count_)
        {
            square_ground_wall_left_distance[count_ - 1] = Mathf.Infinity;
        }
        for (int count_ = square_ground_wall_right_distance.Length; count_ > 0; --count_)
        {
            square_ground_wall_right_distance[count_ - 1] = Mathf.Infinity;
        }


        if (set_befor_col && do_dummy_transform_position_to_set_execute)
        {
            check_change_dummy_transform_position(dummy_transform_position_to_set);
            do_dummy_transform_position_to_set_execute = false;
            dummy_transform_position = dummy_transform_position_to_set;
        }

        if (ground == false) collision();

        if (set_befor_col == false && do_dummy_transform_position_to_set_execute)
        {
            do_dummy_transform_position_to_set_execute = false;
            dummy_transform_position = dummy_transform_position_to_set;
        }
        insertposition();
    }

    void collision()
    {
        count = grounds.Length;
        //変数準備
        //int count = grounds.Length;
        check = 0;
        is_y_0 = false;
        is_x_0 = false;

        ////a1：右上の点の比例定数
        ////a2：左上の点の比例定数
        ////a3：右下の点の比例定数
        ////a4：左下の点の比例定数

        //float a1 = 0;
        //float a2 = 0;
        //float a3 = 0;
        //float a4 = 0;

        ////b1：右上の点のb
        ////b2：左上の点のb
        ////b3：右下の点のb
        ////b4：左下の点のb

        //float b1 = 0;
        //float b2 = 0;
        //float b3 = 0;
        //float b4 = 0;

        if (dummy_transform_position.x == befor_transform_position.x)
        {
            b1 = scriptcol_y.x;
            a1 = 0;

            b2 = scriptcol_y.x;
            a2 = 0;

            b3 = scriptcol_y.y;
            a3 = 0;

            b4 = scriptcol_y.y;
            a4 = 0;

            check += 1;
            is_y_0 = true;
        }
        if (dummy_transform_position.y == befor_transform_position.y)
        {
            b1 = scriptcol_x.x;
            a1 = -1;

            b2 = scriptcol_x.y;
            a2 = -1;

            b3 = scriptcol_x.x;
            a3 = -1;

            b4 = scriptcol_x.y;
            a4 = -1;

            check += 1;
            is_x_0 = true;
            //Debug.Log("trueed");
        }

        if (check == 0)
        {
            a1 = (scriptcol_y.x - befor_scriptcol_y.x) / (scriptcol_x.x - befor_scriptcol_x.x);
            a2 = (scriptcol_y.x - befor_scriptcol_y.x) / (scriptcol_x.y - befor_scriptcol_x.y);
            a3 = (scriptcol_y.y - befor_scriptcol_y.y) / (scriptcol_x.x - befor_scriptcol_x.x);
            a4 = (scriptcol_y.y - befor_scriptcol_y.y) / (scriptcol_x.y - befor_scriptcol_x.y);

            b1 = scriptcol_y.x - (a1 * scriptcol_x.x);
            b2 = scriptcol_y.x - (a2 * scriptcol_x.y);
            b3 = scriptcol_y.y - (a3 * scriptcol_x.x);
            b4 = scriptcol_y.y - (a4 * scriptcol_x.y);

            is_x_0 = false;
        }

        motion motion_processing;
        Vector2 motion_TransSize;

        while (count > 0)
        {
            motion_processing = grounds[count - 1].GetComponent<motion>();
            motion_TransSize = grounds[count - 1].transform.localScale;

            square_ground_wall_left[count - 1] = false;
            square_ground_wall_right[count - 1] = false;
            square_ground_wall_up[count - 1] = false;
            square_ground_wall_down[count - 1] = false;

            //if (Input.GetKey(KeyCode.D) && debuging) Debug.Log(motion_processing + "" + Mathf.Approximately(motion_processing.scriptcol_x.x, scriptcol_x.y));

            //{//動いていないときの判定//使ってない
            //    if (dummy_transform_position.x == befor_transform_position.x && dummy_transform_position.y == befor_transform_position.x)
            //    {
            //        Debug.Log("abcde");
            //        if (square_ground_wall_right[count - 1] == false && scriptcol_x.x >= motion_processing.scriptcol_x.y && scriptcol_x.y <= motion_processing.scriptcol_x.x && dummy_transform_position.x < motion_processing.dummy_transform_position.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//右側
            //        {
            //            dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
            //            square_ground_wall_right[count - 1] = true;
            //            Debug.Log("adc");
            //        }
            //        else square_ground_wall_right[count - 1] = false;

            //        if (square_ground_wall_left[count - 1] == false && scriptcol_x.y <= motion_processing.scriptcol_x.x && scriptcol_x.x >= motion_processing.scriptcol_x.y && dummy_transform_position.x > motion_processing.dummy_transform_position.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//左側
            //        {
            //            dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
            //            square_ground_wall_left[count - 1] = true;
            //            //Debug.Log("working");//問題なし
            //        }
            //        else
            //        {
            //            square_ground_wall_left[count - 1] = false;
            //            //Debug.Log("falsed");
            //        }
            //        //Debug.Log("no move");//右押すとここが動かない(移動してないのに)
            //    }
            //    //else Debug.Log(dummy_transform_position + "," + befor_transform_position);
            //}


            {//動いてないとき用
                //左右
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_right[count - 1] && (!Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y) || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.y))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//右側ぶつかる
                {
                    square_ground_wall_right[count - 1] = false;
                    Debug.Log("waaah");
                }
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_left[count - 1] && (!Mathf.Approximately(scriptcol_x.y,motion_processing.scriptcol_x.x) || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.x))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//左側ぶつかる
                {                                                                                                   //scriptcol_x.y != motion_processing.scriptcol_x.x
                    square_ground_wall_left[count - 1] = false;
                    Debug.Log("cc" + (motion_processing.scriptcol_x.x - scriptcol_x.y));
                }

                //上下
                if (dummy_transform_position.y == befor_transform_position.y && square_ground_wall_up[count - 1] && (!Mathf.Approximately(scriptcol_y.x, motion_processing.scriptcol_y.y) || scriptcol_x.x < motion_processing.scriptcol_x.y || scriptcol_x.y > motion_processing.scriptcol_x.x))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//右側ぶつかる
                {
                    square_ground_wall_up[count - 1] = false;
                    Debug.Log("1up");
                }
                if (dummy_transform_position.y == befor_transform_position.y && square_ground_wall_down[count - 1] && (!Mathf.Approximately(scriptcol_y.y, motion_processing.scriptcol_y.x) || scriptcol_x.x < motion_processing.scriptcol_x.y || scriptcol_x.y > motion_processing.scriptcol_x.y))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//下側ぶつかる
                {
                    square_ground_wall_down[count - 1] = false;
                    Debug.Log("c");
                }
            }

            
            //player_pos();

            if (square_ground_wall_right[count - 1] && square_ground_wall_up[count - 1] == false && square_ground_wall_down[count - 1] == false && (motion_processing.scriptcol_x.y < scriptcol_x.x))//右側
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                square_ground_wall_right[count - 1] = true;
            }
            if (square_ground_wall_left[count - 1] && square_ground_wall_up[count - 1] == false && square_ground_wall_down[count - 1] == false && motion_processing.scriptcol_x.x > scriptcol_x.y)//左側
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                square_ground_wall_left[count - 1] = true;
                Debug.Log("1");
            }
            if (square_ground_wall_up[count - 1] && square_ground_wall_left[count - 1] == false && square_ground_wall_right[count - 1] == false && motion_processing.scriptcol_y.y < scriptcol_y.x)//上側
            {
                //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.y));
                square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                square_ground_wall_up[count - 1] = true;
            }
            if (square_ground_wall_down[count - 1] && square_ground_wall_left[count - 1] == false && square_ground_wall_right[count - 1] == false && motion_processing.scriptcol_y.x > scriptcol_y.y)//下側
            {
                //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.x, false));
                square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                square_ground_wall_down[count - 1] = true;
            }
            

            


            if (is_x_0 == true && is_y_0 == false)//X座標のみ
            {
                //右でも左でも

                if (dummy_transform_position.x - befor_transform_position.x > 0)//右
                {
                    if ((Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y) || scriptcol_x.x > motion_processing.scriptcol_x.y) && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
                    {
                        square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                        //square_ground_wall_right_distance_object[count - 1] = grounds[count - 1];
                        square_ground_wall_right[count - 1] = true;
                    }
                    else square_ground_wall_right[count - 1] = false;
                }

                if (dummy_transform_position.x - befor_transform_position.x < 0)//左
                {
                    if ((Mathf.Approximately(scriptcol_x.y, motion_processing.scriptcol_x.x) || scriptcol_x.y < motion_processing.scriptcol_x.x) && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
                    {
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        //square_ground_wall_right_distance_object[count - 1] = grounds[count - 1];
                        square_ground_wall_left[count - 1] = true;
                        Debug.Log("2");
                    }
                    else square_ground_wall_left[count - 1] = false;
                }



                //373行目と同じ //if (scriptcol_x.x >= motion_processing.scriptcol_x.y && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && touch_left == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//右側ぶつかる
                //{
                //    //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //    if (scriptcol_x.x > motion_processing.scriptcol_x.y && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y) set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                //    square_ground_wall_right[count - 1] = true;
                //    Debug.Log("touch-right" + touch_left + (dummy_transform_position.x + (motion_processing.scriptcol_x.y + -scriptcol_x.x)));
                //}
                //else
                //{
                //    square_ground_wall_right[count - 1] = false;
                //    Debug.Log("falsed" + debugcount);
                //    Debug.Log((scriptcol_x.x >= motion_processing.scriptcol_x.y) + "" + (befor_scriptcol_x.x <= motion_processing.scriptcol_x.y) + (touch_left == false) + (scriptcol_y.x > motion_processing.scriptcol_y.y) + (scriptcol_y.y < motion_processing.scriptcol_y.x));
                //    debugcount += 1;
                //}

                //上に同じく385行目と同じif (scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//左側ぶつかる
                //{
                //    //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                //    //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                //    square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                //    square_ground_wall_left[count - 1] = true;
                //    //times_touching_left = 0;
                //    Debug.Log("a");
                //}
                //else
                //{
                //    square_ground_wall_left[count - 1] = false;
                //}

                //上下の解除
                //if (square_ground_wall_up[count - 1] && (motion_processing.scriptcol_y.y != scriptcol_y.y || motion_processing.scriptcol_x.x < scriptcol_x.y || motion_processing.scriptcol_x.y > scriptcol_x.x))
                //{
                //    square_ground_wall_up[count - 1] = false;
                //}

                //touching_something = true;
            }
            else if (is_y_0 == false)
            {
                
                //touching_something = true;
                //Vector2 local_savepos;
                //Vector2 movement_fordecide_1;
                //Vector2 movement_fordecide_2;

                //- 1次関数の等式変形 -

                //y = ax + b
                //y / x = a + b
                //1 / x = (a + b) / y

                //y = ax + b
                //y - b = ax
                //(y - b) / a = x
                //x = (y - b) / a

                set_cols();

                {
                    if (motion_processing.scriptcol_y.y > befor_scriptcol_y.x && motion_processing.scriptcol_y.y <= scriptcol_y.x && (motion_processing.scriptcol_y.y - b1) / a1 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b1) / a1 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//先にぶつかったのは横か縦か-相手の下面-(a1/右側上)
                    {
                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//進む方向が正
                            {
                                if ((motion_processing.scriptcol_y.y - b1) / a1 > befor_scriptcol_x.x && (motion_processing.scriptcol_y.y - b1) / a1 <= scriptcol_x.x)
                                {
                                    //dummy_transform_position += new Vector3(((motion_processing.scriptcol_y.y - b1) / a1) - dummy_transform_position.x, (a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1) - dummy_transform_position.y, 0);
                                    //Debug.Log("aaaa" + (motion_processing.scriptcol_y.y - b1) / a1);

                                    if (stop_when_this_collide)
                                    {
                                        //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b1) / a1), true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                        square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                    }
                                    else
                                    {
                                        //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                        square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                    }
                                    square_ground_wall_up[count - 1] = true;
                                }
                            }
                            else if ((motion_processing.scriptcol_y.y - b1) / a1 < befor_scriptcol_x.x && (motion_processing.scriptcol_y.y - b1) / a1 >= scriptcol_x.x)//進む方向が負
                            {
                                //Debug.Log("ea3");

                                if (stop_when_this_collide)
                                {
                                    //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b1) / a1), true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                    square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                }
                                else
                                {
                                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                    square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                }
                                square_ground_wall_up[count - 1] = true;
                            }

                            //if (square_ground_wall_up[count - 1])
                            //{
                            //    //Debug.Log("ea2");
                            //    //dummy_transform_position += new Vector3((((motion_processing.scriptcol_y.y - scriptcol_y.x) - b1) / a1), motion_processing.scriptcol_y.y - scriptcol_y.x, 0);
                            //}
                        }//ぶつかったかどうか、移動はまだ
                    }
                    else if(motion_processing.scriptcol_y.x < befor_scriptcol_y.y && motion_processing.scriptcol_y.x >= scriptcol_y.y && (motion_processing.scriptcol_y.x - b3) / a3 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.x - b3) / a3 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y > 0)//先にぶつかったのは横か縦か-相手の下面-(a3/右側下)
                    {
                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//進む方向が正
                            {
                                if ((motion_processing.scriptcol_y.x - b3) / a3 > befor_scriptcol_x.x && (motion_processing.scriptcol_y.x - b3) / a3 <= scriptcol_x.x)
                                {
                                    if (stop_when_this_collide)
                                    {
                                        //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b1) / a1), true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                        square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                    }
                                    else
                                    {
                                        //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                        square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                    }
                                    square_ground_wall_down[count - 1] = true;
                                }
                            }
                            else if ((motion_processing.scriptcol_y.x - b3) / a3 < befor_scriptcol_x.x && (motion_processing.scriptcol_y.x - b3) / a3 >= scriptcol_x.x)//進む方向が負
                            {
                                //Debug.Log("ea3");

                                if (stop_when_this_collide)
                                {
                                    //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b1) / a1), true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                }
                                else
                                {
                                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1));
                                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                }
                                square_ground_wall_down[count - 1] = true;
                            }/////////////////////////////////////////////////////////////////////////////////__6/12__////////////////////////////////////////////////////////////////////

                            //if (square_ground_wall_up[count - 1])
                            //{
                            //    //Debug.Log("ea2");
                            //    //dummy_transform_position += new Vector3((((motion_processing.scriptcol_y.y - scriptcol_y.x) - b1) / a1), motion_processing.scriptcol_y.y - scriptcol_y.x, 0);
                            //}
                        }//ぶつかったかどうか、移動はまだ
                    }
                    else if (motion_processing.scriptcol_y.y > befor_scriptcol_y.x && motion_processing.scriptcol_y.y <= scriptcol_y.x && (motion_processing.scriptcol_y.y - b2) / a2 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b2) / a2 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//先にぶつかったのは横か縦か-相手の下面-(a2/左側上)
                    {
                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//進む方向が正
                            {
                                if ((motion_processing.scriptcol_y.y - b2) / a2 > befor_scriptcol_x.x && (motion_processing.scriptcol_y.y - b2) / a2 <= scriptcol_x.x)
                                {
                                    //dummy_transform_position += new Vector3(((motion_processing.scriptcol_y.y - b1) / a1) - dummy_transform_position.x, (a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1) - dummy_transform_position.y, 0);
                                    //Debug.Log("aaaa" + (motion_processing.scriptcol_y.y - b2) / a2);

                                    if (stop_when_this_collide)
                                    {
                                        //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b2) / a2), true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                        square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                    }
                                    else
                                    {
                                        //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                        square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                    }
                                    square_ground_wall_up[count - 1] = true;
                                }
                            }
                            else if ((motion_processing.scriptcol_y.y - b2) / a2 < befor_scriptcol_x.x && (motion_processing.scriptcol_y.y - b2) / a2 >= scriptcol_x.x)//進む方向が負
                            {
                                //Debug.Log("ea3");

                                if (stop_when_this_collide)
                                {
                                    //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b2) / a2), true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                    square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                }
                                else
                                {
                                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                    square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                                }
                                square_ground_wall_up[count - 1] = true;
                            }
                        }
                    }
                    else if (motion_processing.scriptcol_y.x < befor_scriptcol_y.y && motion_processing.scriptcol_y.x >= scriptcol_y.y && (motion_processing.scriptcol_y.x - b4) / a4 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.x - b4) / a4 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y > 0)//先にぶつかったのは横か縦か-相手の下面-(a4/左側下)
                    {
                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            //
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//進む方向が正
                            {
                                //
                                if ((motion_processing.scriptcol_y.x - b4) / a4 > befor_scriptcol_x.y && (motion_processing.scriptcol_y.x - b4) / a4 <= scriptcol_x.y)
                                {
                                    if (stop_when_this_collide)
                                    {
                                        //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b2) / a2), true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                        square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                    }
                                    else
                                    {
                                        //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                        square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                    }
                                    square_ground_wall_down[count - 1] = true;
                                }
                            }
                            else if ((motion_processing.scriptcol_y.x - b4) / a4 < befor_scriptcol_x.y && (motion_processing.scriptcol_y.x - b4) / a4 >= scriptcol_x.y)//進む方向が負
                            {
                                //Debug.Log("ea3");

                                if (stop_when_this_collide)
                                {
                                    //set_dummy_transform_position(true, change_col_to_pos_x((motion_processing.scriptcol_y.y - b2) / a2), true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                }
                                else
                                {
                                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(a2 * ((motion_processing.scriptcol_y.y - b2) / a2) + b2));
                                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                                }
                                square_ground_wall_down[count - 1] = true;
                            }
                        }
                    }
                }

                set_cols();

                if (motion_TransSize.y >= mysize.y || Mathf.Approximately(motion_TransSize.y, mysize.y))//相手のほうが大きいまたは同じサイズ
                {
                    if ((a1 * motion_processing.scriptcol_x.y + b1) >= motion_processing.scriptcol_y.y && (a1 * motion_processing.scriptcol_x.y + b1) <= motion_processing.scriptcol_y.x && square_ground_wall_up[count - 1] == false && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_x.x >= motion_processing.scriptcol_x.y)//右側上ぶつかる
                    {
                        Debug.Log("ea");



                        //if ()
                        {
                            //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                            //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);//true, change_col_to_pos_y(a1 * motion_processing.scriptcol_x.y + b1));
                            square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                            square_ground_wall_right[count - 1] = true;
                        }
                    }


                    set_cols();

                    if ((a3 * motion_processing.scriptcol_x.y + b3) >= motion_processing.scriptcol_y.y && (a3 * motion_processing.scriptcol_x.y + b3) <= motion_processing.scriptcol_y.x && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_x.x >= motion_processing.scriptcol_x.y)//右側下ぶつかる
                    {
                        //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                        //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                        square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                        square_ground_wall_right[count - 1] = true;
                    }

                    set_cols();

                    if ((a2 * motion_processing.scriptcol_x.x + b2) >= motion_processing.scriptcol_y.y && (a2 * motion_processing.scriptcol_x.x + b2) <= motion_processing.scriptcol_y.x && square_ground_wall_up[count - 1] == false && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_x.y <= motion_processing.scriptcol_x.x)//左側上ぶつかる
                    {
                        //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                        //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        square_ground_wall_left[count - 1] = true;
                        Debug.Log("working1");
                    }

                    set_cols();

                    if ((a4 * motion_processing.scriptcol_x.x + b4) >= motion_processing.scriptcol_y.y && (a4 * motion_processing.scriptcol_x.x + b4) <= motion_processing.scriptcol_y.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_x.y <= motion_processing.scriptcol_x.x)//左側下ぶつかる
                    {
                        //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                        //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        square_ground_wall_left[count - 1] = true;
                        Debug.Log("3");
                    }
                }
                else//自分のほうが大きい
                {
                    //左
                    if ((a2 * motion_processing.scriptcol_x.x + b2) > motion_processing.scriptcol_y.y && (a4 * motion_processing.scriptcol_x.x + b4) < motion_processing.scriptcol_y.x && motion_processing.scriptcol_x.x < befor_scriptcol_x.y && motion_processing.scriptcol_x.x > scriptcol_x.y)
                    {
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        square_ground_wall_left[count - 1] = true;
                        Debug.Log("4");
                    }

                    //右
                    if ((a1 * motion_processing.scriptcol_x.y + b1) > motion_processing.scriptcol_y.y && (a3 * motion_processing.scriptcol_x.y + b3) < motion_processing.scriptcol_y.x && motion_processing.scriptcol_x.y > befor_scriptcol_x.x && motion_processing.scriptcol_x.y < scriptcol_x.x)
                    {
                        square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                        square_ground_wall_right[count - 1] = true;
                    }
                }
            }
            else if (is_y_0 == true && dummy_transform_position.y != befor_transform_position.y && dummy_transform_position.x == befor_transform_position.x)//上下移動のみ
            {
                if (motion_processing.scriptcol_y.y > befor_scriptcol_y.x && motion_processing.scriptcol_y.y <= scriptcol_y.x && motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_x.x > scriptcol_x.y)//&& (motion_processing.scriptcol_y.y - b1) / a1 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b1) / a1 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//先にぶつかったのは横か縦か-相手の下面-
                {
                    Debug.Log("ea1");
                    //dummy_transform_position += new Vector3(0, motion_processing.scriptcol_y.y - scriptcol_y.x, 0);
                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.y));//a1 * dummy_transform_position.x + b1);
                    square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                    square_ground_wall_up[count - 1] = true;
                    //Debug.Log("ssss");
                }
                else
                {
                    square_ground_wall_up[count - 1] = false;
                }

                if (motion_processing.scriptcol_y.x < befor_scriptcol_y.y && motion_processing.scriptcol_y.x >= scriptcol_y.y && motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_x.x > scriptcol_x.y)//先にぶつかったのは横か縦か-相手の上面-
                {
                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.x, false));
                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                    square_ground_wall_down[count - 1] = true;
                }
            }

            set_cols();

            if (square_ground_wall_right[count - 1] == false && scriptcol_x.x == motion_processing.scriptcol_x.y && motion_processing.scriptcol_y.x > scriptcol_y.y && motion_processing.scriptcol_y.y < scriptcol_y.x)//右ぶつかる
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                square_ground_wall_right[count - 1] = true;
                Debug.Log("hello-");
            }
            if (square_ground_wall_left[count - 1] == false && scriptcol_x.y == motion_processing.scriptcol_x.x && motion_processing.scriptcol_y.x > scriptcol_y.y && motion_processing.scriptcol_y.y < scriptcol_y.x)//左ぶつかる
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                square_ground_wall_left[count - 1] = true;
                //Debug.Log("5");
            }
            if (square_ground_wall_up[count - 1] == false && scriptcol_y.x == motion_processing.scriptcol_y.y && motion_processing.scriptcol_x.x > scriptcol_x.y && motion_processing.scriptcol_x.y < scriptcol_x.x)
            {
                square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                square_ground_wall_up[count - 1] = true;
            }
            if (square_ground_wall_down[count - 1] == false && scriptcol_y.y == motion_processing.scriptcol_y.x && motion_processing.scriptcol_x.x > scriptcol_x.y && motion_processing.scriptcol_x.y < scriptcol_x.x)
            {
                square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                square_ground_wall_down[count - 1] = true;
            }

            count -= 1;
        }

        insertposition();

        {
            //int true_ = 0;

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_right, true))
            {//右側

                float distance = Mathf.Min(square_ground_wall_right_distance);
                int count_;

                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_right_distance, distance);

                    if (!only_whether_touch)
                    {
                        set_dummy_transform_position(true, change_col_to_pos_x(grounds[count_].GetComponent<motion>().scriptcol_x.y), false, 0);
                        touch_right = true;
                    }

                    if (0 < movementvalue.x)
                    {
                        movementvalue.x = 0;
                    }
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_right, true))
            {
                touch_right = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_left, true))
            {//左側

                float distance = Mathf.Min(square_ground_wall_left_distance);
                int count_;
                //Debug.Log(square_ground_wall_left_distance + "" + distance);
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_left_distance, distance);

                    if (!only_whether_touch)
                    {
                        set_dummy_transform_position(true, change_col_to_pos_x(grounds[count_].GetComponent<motion>().scriptcol_x.x, false), false, 0);
                        touch_left = true;
                    }

                    if (movementvalue.x < 0)
                    {
                        movementvalue.x = 0;
                    }
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_left, true))
            {
                touch_left = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_up, true))
            {//上側

                float distance = Mathf.Min(square_ground_wall_up_distance);
                int count_;
                
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_up_distance, distance);

                    if (!only_whether_touch)
                    {
                        set_dummy_transform_position(false, 0, true, change_col_to_pos_y(grounds[count_].GetComponent<motion>().scriptcol_y.y));
                        touch_up = true;
                    }

                    if (0 < movementvalue.y)
                    {
                        movementvalue.y = 0;
                    }
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_up, true))
            {
                touch_up = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_down, true))
            {//下側

                float distance = Mathf.Min(square_ground_wall_down_distance);
                int count_;
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_down_distance, distance);

                    if (!only_whether_touch)
                    {
                        set_dummy_transform_position(false, 0, true, change_col_to_pos_y(grounds[count_].GetComponent<motion>().scriptcol_y.x, false));
                        touch_down = true;
                    }

                    if (0 > movementvalue.y)
                    {
                        movementvalue.y = 0;
                    }
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_down, true))
            {
                touch_down = false;
            }

            if (touch_down || touch_left || touch_right || touch_up) touching_something = true;
            else touching_something = false;

            for (int count_ = touching_down.Length; count_ > 0; --count_)
            {
                touching_down[count_ - 1] = null;
            }
            for (int count_ = touching_up.Length; count_ > 0; --count_)
            {
                touching_up[count_ - 1] = null;
            }
            for (int count_ = touching_right.Length; count_ > 0; --count_)
            {
                touching_right[count_ - 1] = null;
            }
            for (int count_ = touching_left.Length; count_ > 0; --count_)
            {
                touching_left[count_ - 1] = null;
            }

            int putcount = 0;
            int storecount = 0;


            for (int count__ = square_ground_wall_down.Length; count__ > 0; --count__)
            {
                if (square_ground_wall_down[count__ - 1]) putcount += 1;
            }

            touching_down = new GameObject[putcount];
            putcount = 0;

            for (int count__ = square_ground_wall_up.Length; count__ > 0; --count__)
            {
                if (square_ground_wall_up[count__ - 1]) putcount += 1;
            }

            touching_up = new GameObject[putcount];
            putcount = 0;

            for (int count__ = square_ground_wall_right.Length; count__ > 0; --count__)
            {
                if (square_ground_wall_right[count__ - 1]) putcount += 1;
            }

            touching_right = new GameObject[putcount];
            putcount = 0;

            for (int count__ = square_ground_wall_left.Length; count__ > 0; --count__)
            {
                if (square_ground_wall_left[count__ - 1]) putcount += 1;
            }

            touching_left = new GameObject[putcount];
            putcount = 0;


            if (touching_something)
            {
                for (int cc = square_ground_wall_down.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_down[cc - 1])
                    {
                        touching_down[putcount] = grounds[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                for (int cc = square_ground_wall_up.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_up[cc - 1])
                    {
                        touching_up[putcount] = grounds[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                for (int cc = square_ground_wall_left.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_left[cc - 1])
                    {
                        touching_left[putcount] = grounds[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                for (int cc = square_ground_wall_right.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_right[cc - 1])
                    {
                        touching_right[putcount] = grounds[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                touching = new GameObject[storecount];

                if (storecount > 0)
                {
                    storecount = 0;
                    for (int cc = touching_down.Length; cc > 0; --cc)
                    {
                        //if (touching_down[cc - 1] != null) 
                        touching[storecount + cc - 1] = touching_down[cc - 1];
                    }

                    storecount += touching_down.Length;

                    for (int cc = touching_up.Length; cc > 0; --cc)
                    {
                        touching[storecount + cc - 1] = touching_up[cc - 1];
                    }

                    storecount += touching_up.Length;

                    for (int cc = touching_right.Length; cc > 0; --cc)
                    {
                        touching[storecount + cc - 1] = touching_right[cc - 1];
                    }

                    storecount += touching_right.Length;

                    for (int cc = touching_left.Length; cc > 0; --cc)
                    {
                        touching[storecount + cc - 1] = touching_left[cc - 1];
                    }
                }
                //storecount += touching_left.Length;
            }
            else touching = new GameObject[0];
        }
    }

    public void movement(Vector2 movementvalueforset, bool set)
    {
        bool numplus_x = true;

        if (set == true)
        {
            set = false;
            movementvalue = movementvalueforset;
            if (movementvalueforset.x > 0)
            {
                numplus_x = true;
            }
            else numplus_x = false;

            //if (movementvalue.x == 0.3f) Debug.Log("go right");
        }

        if (touch_left && movementvalue.x < 0) movementvalue.x = 0;
        if (touch_right && movementvalue.x > 0) debuging.Log("movementvalue.x = 0");
        if (touch_down && movementvalue.y < 0) movementvalue.y = 0;
        if (touch_up && movementvalue.y > 0) movementvalue.y = 0;

        dummy_transform_position += new Vector3(movementvalue.x, movementvalue.y, 0);
        if (Air_resistance != 0)
        {
            bool plus;

            if (Air_resistance > 0) plus = true;
            else plus = false;

            if (plus == true) movementvalue.x += -Air_resistance;
            else movementvalue.x += Air_resistance;

            if (plus == true && numplus_x == true && movementvalue.x < 0) movementvalue.x = 0;
            if (plus == true && numplus_x == false && movementvalue.x > 0) movementvalue.x = 0;
            //if (plus == false && movementvalue.x > 0) movementvalue.x = 0;
        }

        if (gravity != 0)
        {
            movementvalue.y -= gravity;
        }
    }

    void insertposition()
    {
        befor_scriptcol_x = scriptcol_x;
        befor_scriptcol_y = scriptcol_y;
        
        if (touching_something)
        {
            if ((touch_down && dummy_transform_position.y < befor_transform_position.y) || (touch_up && dummy_transform_position.y > befor_transform_position.y)) dummy_transform_position.y = befor_transform_position.y;
            if ((touch_right && dummy_transform_position.x > befor_transform_position.x) || (touch_left && dummy_transform_position.x < befor_transform_position.x)) dummy_transform_position.x = befor_transform_position.x;
        }
        transform.position = dummy_transform_position;
        befor_transform_position = dummy_transform_position;
    }

    void set_cols()
    {
        check = 0;
        is_y_0 = false;
        is_x_0 = false;

        if (dummy_transform_position.x == befor_transform_position.x)
        {
            b1 = scriptcol_y.x;
            a1 = 0;

            b2 = scriptcol_y.x;
            a2 = 0;

            b3 = scriptcol_y.y;
            a3 = 0;

            b4 = scriptcol_y.y;
            a4 = 0;

            check += 1;
            is_y_0 = true;
        }
        if (dummy_transform_position.y == befor_transform_position.y)
        {
            b1 = scriptcol_x.x;
            a1 = -1;

            b2 = scriptcol_x.y;
            a2 = -1;

            b3 = scriptcol_x.x;
            a3 = -1;

            b4 = scriptcol_x.y;
            a4 = -1;

            check += 1;
            is_x_0 = true;
            //Debug.Log("trueed");
        }

        if (check == 0)
        {
            a1 = (scriptcol_y.x - befor_scriptcol_y.x) / (scriptcol_x.x - befor_scriptcol_x.x);
            a2 = (scriptcol_y.x - befor_scriptcol_y.x) / (scriptcol_x.y - befor_scriptcol_x.y);
            a3 = (scriptcol_y.y - befor_scriptcol_y.y) / (scriptcol_x.x - befor_scriptcol_x.x);
            a4 = (scriptcol_y.y - befor_scriptcol_y.y) / (scriptcol_x.y - befor_scriptcol_x.y);

            b1 = scriptcol_y.x - (a1 * scriptcol_x.x);
            b2 = scriptcol_y.x - (a2 * scriptcol_x.y);
            b3 = scriptcol_y.y - (a3 * scriptcol_x.x);
            b4 = scriptcol_y.y - (a4 * scriptcol_x.y);

            is_x_0 = false;
        }
    }

    public void change_dummy_transform_position(bool absolute_x, float x, bool absolute_y, float y, bool absolute_z = false, float z = 0, bool movinig_without_col = false)
    {
        dummy_transform_position_to_set = dummy_transform_position;

        //set x
        if (absolute_x) dummy_transform_position_to_set.x = x;
        else dummy_transform_position_to_set.x += x;

        //set y
        if (absolute_y) dummy_transform_position_to_set.y = y;
        else dummy_transform_position_to_set.y += y;

        //set z
        if (absolute_z) dummy_transform_position_to_set.z = z;
        else dummy_transform_position_to_set.z += z;

        set_befor_col = !movinig_without_col;
        do_dummy_transform_position_to_set_execute = true;
    }

    public bool is_touching(GameObject something)
    {
        if (ArrayUtility.Contains(touching, something))
        {
            return true;
        }
        else return false;
    }

    public bool searching_object_withTag(string tag)
    {
        int c = 0;
        for (int count__ = touching.Length; count__ > 0; --count__)
        {
            if (touching[count__ - 1].tag == tag) c += 1;
        }
        if (c > 0) return true;
        else return false;
    }

    public GameObject touching_Object(int num)
    {
        return touching[num];
    }

    void set_dummy_transform_position(bool absolute_x, float x, bool absolute_y, float y, bool absolute_z = false, float z = 0)//, bool movinig_without_col = false)
    {
        //set x
        if (absolute_x) dummy_transform_position.x = x;
        else dummy_transform_position.x += x;
        //set y
        if (absolute_y) dummy_transform_position.y = y;
        else dummy_transform_position.y += y;

        //set z
        if (absolute_z) dummy_transform_position.z = z;
        else dummy_transform_position.z += z;
    }

    void check_change_dummy_transform_position(Vector3 pos)
    {
        if (touch_down && dummy_transform_position.y > dummy_transform_position_to_set.y)
        {
            dummy_transform_position_to_set.y = dummy_transform_position.y;
        }

        if (touch_up && dummy_transform_position.y < dummy_transform_position_to_set.y)
        {
            dummy_transform_position_to_set.y = dummy_transform_position.y;
        }

        if (touch_left && dummy_transform_position.x > dummy_transform_position_to_set.x)
        {
            dummy_transform_position_to_set.x = dummy_transform_position.x;
        }

        if (touch_right && dummy_transform_position.x < dummy_transform_position_to_set.x)
        {
            dummy_transform_position_to_set.x = dummy_transform_position.x;
        }
    }

    //    (dummy_transform_position.x + (box2d.size.x* (transform.localScale.x* 0.5f)))

    //dummy.x + box2d.s.x(0.5*trans.s.x)=a
    //dummy.x + box2d.s.x((trans.s.x)/2)=a
    //dummy.x + ((box2d.s.x * trans.s.x)/2)=a
    //dummy.x = a - ((box2d.s.x * trans.s.x)/2)

    public float change_col_to_pos_x(float colx, bool is_x = true)
    {
        //Debug.Log("++" + is_x);
        if (is_x) return colx - ((box2d.size.x * transform.localScale.x) / 2);
        else return colx + ((box2d.size.x * transform.localScale.x) / 2);
    }

    public float change_col_to_pos_y(float coly, bool is_x = true)
    {
        if (is_x) return coly - ((box2d.size.y * transform.localScale.y) / 2);
        else return coly - ((-box2d.size.y * transform.localScale.y) / 2);
    }

    void colcount()
    {
        square_ground_wall_down = new bool[grounds.Length];

        square_ground_wall_up = new bool[grounds.Length];

        square_ground_wall_right = new bool[grounds.Length];

        square_ground_wall_left = new bool[grounds.Length];

        square_ground_wall_down_distance = new float[grounds.Length];

        square_ground_wall_up_distance = new float[grounds.Length];

        square_ground_wall_right_distance = new float[grounds.Length];

        square_ground_wall_left_distance = new float[grounds.Length];

        touching = new GameObject[grounds.Length];

        touching_down = new GameObject[grounds.Length];

        touching_up = new GameObject[grounds.Length];

        touching_right = new GameObject[grounds.Length];

        touching_left = new GameObject[grounds.Length];
    }

    void drowline(Vector3 start, Vector3 end)
    {
        Debug.DrawLine(start, end, Color.red, 2,false);
    }
}
