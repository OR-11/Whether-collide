using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class motion : MonoBehaviour
{
    public bool debuging;

    public bool ground;

    public bool stop_when_this_collide;

    public float Air_resistance;
    public float gravity;

    public Vector2 scriptcol_x;
    public Vector2 scriptcol_y;

    public Vector2 befor_scriptcol_x;
    public Vector2 befor_scriptcol_y;

    public Vector2 movementvalue;

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
    //���W�̍�(�E/��:X���W | ��/��:Y���W)�̐�Βl���i�[����\��
    public float[] square_ground_wall_right_distance;

    public float[] square_ground_wall_left_distance;

    public float[] square_ground_wall_up_distance;

    public float[] square_ground_wall_down_distance;
    //Mathf.Abs(a) a����Βl�ɂȂ��Ė߂��Ă���
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

    int times_touching_right;

    int times_touching_left;

    int times_touching_up;

    int times_touching_down;

    int debugcount;

    //����R���_�[����
    //�ϐ�����
    int count;// = grounds.Length;
    int check = 0;
    bool is_y_0 = false;
    bool is_x_0 = false;

    //a1�F�E��̓_�̔��萔
    //a2�F����̓_�̔��萔
    //a3�F�E���̓_�̔��萔
    //a4�F�����̓_�̔��萔

    float a1 = 0;
    float a2 = 0;
    float a3 = 0;
    float a4 = 0;

    //b1�F�E��̓_��b
    //b2�F����̓_��b
    //b3�F�E���̓_��b
    //b3�F�����̓_��b

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
        }
    }

    private void FixedUpdate()
    {
        dummy_transform_position = transform.position;

        if (dummy_transform_position != befor_transform_position) Debug.Log("not same");

        scriptcol_x = new Vector2((dummy_transform_position.x + (box2d.size.x * (transform.localScale.x * 0.5f))), (dummy_transform_position.x + (-box2d.size.x * (transform.localScale.x * 0.5f))));
        scriptcol_y = new Vector2((dummy_transform_position.y + (box2d.size.y * (transform.localScale.y * 0.5f))), (dummy_transform_position.y + (-box2d.size.y * (transform.localScale.y * 0.5f))));

        if (Input.GetKey(KeyCode.Space) && debuging) movement(new Vector2(-10, 0), true);
        else movement(new Vector2(0, 0), false);


        //Array.Fill(square_ground_wall_down_distance, -1);//���Ȃ񂩏o���Ȃ�����for��
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
        //�ϐ�����
        //int count = grounds.Length;
        check = 0;
        is_y_0 = false;
        is_x_0 = false;

        ////a1�F�E��̓_�̔��萔
        ////a2�F����̓_�̔��萔
        ////a3�F�E���̓_�̔��萔
        ////a4�F�����̓_�̔��萔

        //float a1 = 0;
        //float a2 = 0;
        //float a3 = 0;
        //float a4 = 0;

        ////b1�F�E��̓_��b
        ////b2�F����̓_��b
        ////b3�F�E���̓_��b
        ////b3�F�����̓_��b

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

        while (count > 0)
        {
            motion_processing = grounds[count - 1].GetComponent<motion>();

            {//�����Ă��Ȃ��Ƃ��̔���//�g���ĂȂ�
                if (dummy_transform_position.x == befor_transform_position.x && dummy_transform_position.y == befor_transform_position.x)
                {
                    Debug.Log("abcde");
                    if (square_ground_wall_right[count - 1] == false && scriptcol_x.x >= motion_processing.scriptcol_x.y && scriptcol_x.y <= motion_processing.scriptcol_x.x && dummy_transform_position.x < motion_processing.dummy_transform_position.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//�E��
                    {
                        dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                        square_ground_wall_right[count - 1] = true;
                    }
                    else square_ground_wall_right[count - 1] = false;

                    if (square_ground_wall_left[count - 1] == false && scriptcol_x.y <= motion_processing.scriptcol_x.x && scriptcol_x.x >= motion_processing.scriptcol_x.y && dummy_transform_position.x > motion_processing.dummy_transform_position.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//����
                    {
                        dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                        square_ground_wall_left[count - 1] = true;
                        //Debug.Log("working");//���Ȃ�
                    }
                    else
                    {
                        square_ground_wall_left[count - 1] = false;
                        //Debug.Log("falsed");
                    }
                    //Debug.Log("no move");//�E�����Ƃ����������Ȃ�(�ړ����ĂȂ��̂�)
                }
                //else Debug.Log(dummy_transform_position + "," + befor_transform_position);
            }


            {//�����ĂȂ��Ƃ��p
                //���E
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_right[count - 1] && (scriptcol_x.x != motion_processing.scriptcol_x.y || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.y))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//�E���Ԃ���
                {
                    square_ground_wall_right[count - 1] = false;
                    Debug.Log("waaah");
                }
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_left[count - 1] && (scriptcol_x.y != motion_processing.scriptcol_x.x || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.x))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//�����Ԃ���
                {
                    square_ground_wall_left[count - 1] = false;
                    Debug.Log("cc" + (scriptcol_x.y - motion_processing.scriptcol_x.x));
                }

                //�㉺
                if (dummy_transform_position.y == befor_transform_position.y && square_ground_wall_up[count - 1] && (scriptcol_y.x != motion_processing.scriptcol_y.y || scriptcol_x.x < motion_processing.scriptcol_x.y || scriptcol_x.y > motion_processing.scriptcol_x.x))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//�E���Ԃ���
                {
                    square_ground_wall_up[count - 1] = false;
                    Debug.Log("1up");
                }
                if (dummy_transform_position.y == befor_transform_position.y && square_ground_wall_down[count - 1] && (scriptcol_y.y != motion_processing.scriptcol_y.x || scriptcol_x.x < motion_processing.scriptcol_x.y || scriptcol_x.y > motion_processing.scriptcol_x.y))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//�����Ԃ���
                {
                    square_ground_wall_down[count - 1] = false;
                    Debug.Log("c");
                }
            }

            
            //player_pos();

            if (square_ground_wall_right[count - 1] && square_ground_wall_up[count - 1] == false && square_ground_wall_down[count - 1] == false && motion_processing.scriptcol_x.y < scriptcol_x.x)//�E��
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
            }
            if (square_ground_wall_left[count - 1] && square_ground_wall_up[count - 1] == false && square_ground_wall_down[count - 1] == false && motion_processing.scriptcol_x.x > scriptcol_x.y)//����
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
            }
            if (square_ground_wall_up[count - 1] && square_ground_wall_left[count - 1] == false && square_ground_wall_right[count - 1] == false && motion_processing.scriptcol_y.y < scriptcol_y.x)//�㑤
            {
                //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.y));
                square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
            }
            if (square_ground_wall_down[count - 1] && square_ground_wall_left[count - 1] == false && square_ground_wall_right[count - 1] == false && motion_processing.scriptcol_y.x > scriptcol_y.y)//����
            {
                set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.x, false));
            }
            //�Ȃɂ�������?(�����ς�...?)

            


            if (is_x_0 == true && is_y_0 == false)//X���W�̂�
            {
                //�E�ł����ł�

                if (dummy_transform_position.x - befor_transform_position.x > 0)//�E
                {
                    if (scriptcol_x.x >= motion_processing.scriptcol_x.y && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && touch_left == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
                    {
                        square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                        //square_ground_wall_right_distance_object[count - 1] = grounds[count - 1];
                        square_ground_wall_right[count - 1] = true;
                    }
                    else square_ground_wall_right[count - 1] = false;
                }

                if (dummy_transform_position.x - befor_transform_position.x < 0)//��
                {
                    if (scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
                    {
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        //square_ground_wall_right_distance_object[count - 1] = grounds[count - 1];
                        square_ground_wall_left[count - 1] = true;
                    }
                    else square_ground_wall_right[count - 1] = false;
                }



                //373�s�ڂƓ��� //if (scriptcol_x.x >= motion_processing.scriptcol_x.y && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && touch_left == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//�E���Ԃ���
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

                //��ɓ�����385�s�ڂƓ���if (scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//�����Ԃ���
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

                //�㉺�̉���
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

                //- 1���֐��̓����ό` -

                //y = ax + b
                //y / x = a + b
                //1 / x = (a + b) / y

                //y = ax + b
                //y - b = ax
                //(y - b) / a = x
                //x = (y - b) / a

                set_cols();

                //if ((a1 * motion_processing.scriptcol_x.y + b1) >= motion_processing.scriptcol_y.y && (a1 * motion_processing.scriptcol_x.y + b1) <= motion_processing.scriptcol_y.x && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_x.x >= motion_processing.scriptcol_x.y)//��ʂ�����
                {
                    //Debug.Log("hello" + "" + (scriptcol_y.y > befor_scriptcol_y.x) + "" + (scriptcol_y.y <= scriptcol_y.x) + "" + ((motion_processing.scriptcol_y.y - b1) / a1 > motion_processing.scriptcol_x.y) + "" + ((motion_processing.scriptcol_y.y - b1) / a1 < motion_processing.scriptcol_x.x) + ((motion_processing.scriptcol_y.y - b1) / a1) + (befor_transform_position.y - dummy_transform_position.y < 0));
                    //if (motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_y.y < scriptcol_y.x) UnityEditor.EditorApplication.isPaused = true;

                    if (motion_processing.scriptcol_y.y > befor_scriptcol_y.x && motion_processing.scriptcol_y.y <= scriptcol_y.x && (motion_processing.scriptcol_y.y - b1) / a1 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b1) / a1 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//��ɂԂ������͉̂����c��-����̉���-(a1/�E����)
                    {
                        //Debug.Log("he" + (((motion_processing.scriptcol_y.y - b1) / a1) - dummy_transform_position.x));

                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//�i�ޕ�������
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
                            else if ((motion_processing.scriptcol_y.y - b1) / a1 < befor_scriptcol_x.x && (motion_processing.scriptcol_y.y - b1) / a1 >= scriptcol_x.x)//�i�ޕ�������
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
                        }//�Ԃ��������ǂ����A�ړ��͂܂�
                    }
                    else if (motion_processing.scriptcol_y.y > befor_scriptcol_y.x && motion_processing.scriptcol_y.y <= scriptcol_y.x && (motion_processing.scriptcol_y.y - b2) / a2 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b2) / a2 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//��ɂԂ������͉̂����c��-����̉���-(a2/������)
                    {
                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//�i�ޕ�������
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
                            else if ((motion_processing.scriptcol_y.y - b2) / a2 < befor_scriptcol_x.x && (motion_processing.scriptcol_y.y - b2) / a2 >= scriptcol_x.x)//�i�ޕ�������
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
                }

                set_cols();

                if ((a1 * motion_processing.scriptcol_x.y + b1) >= motion_processing.scriptcol_y.y && (a1 * motion_processing.scriptcol_x.y + b1) <= motion_processing.scriptcol_y.x && touch_left == false && square_ground_wall_up[count - 1] == false && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_x.x >= motion_processing.scriptcol_x.y)//�E����Ԃ���
                {
                    //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);//y���W���C������ĂȂ�
                    //square_ground_wall_right[count - 1] = true;
                    //local_savepos = new Vector2(motion_processing.scriptcol_x.y, a1 * motion_processing.scriptcol_x.y + b1);

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

                if ((a3 * motion_processing.scriptcol_x.y + b3) >= motion_processing.scriptcol_y.y && (a3 * motion_processing.scriptcol_x.y + b3) <= motion_processing.scriptcol_y.x && touch_left == false && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_x.x >= motion_processing.scriptcol_x.y)//�E�����Ԃ���
                {
                    //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                    //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                    square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                    square_ground_wall_right[count - 1] = true;
                }

                set_cols();

                if ((a2 * motion_processing.scriptcol_x.x + b2) >= motion_processing.scriptcol_y.y && (a2 * motion_processing.scriptcol_x.x + b2) <= motion_processing.scriptcol_y.x && touch_right == false && square_ground_wall_up[count - 1] == false && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_x.y <= motion_processing.scriptcol_x.x)//������Ԃ���
                {
                    //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                    //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                    square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                    square_ground_wall_left[count - 1] = true;
                    Debug.Log("working1");
                }

                set_cols();

                if ((a4 * motion_processing.scriptcol_x.x + b4) >= motion_processing.scriptcol_y.y && (a4 * motion_processing.scriptcol_x.x + b4) <= motion_processing.scriptcol_y.x && touch_right == false && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_x.y <= motion_processing.scriptcol_x.x)//�������Ԃ���
                {
                    //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                    //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                    square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                    square_ground_wall_left[count - 1] = true;
                }


                
            }
            else if (is_y_0 == true && dummy_transform_position.y != befor_transform_position.y && dummy_transform_position.x == befor_transform_position.x)//�㉺�ړ��̂�
            {
                if (motion_processing.scriptcol_y.y > befor_scriptcol_y.x && motion_processing.scriptcol_y.y <= scriptcol_y.x && motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_x.x > scriptcol_x.y)//&& (motion_processing.scriptcol_y.y - b1) / a1 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b1) / a1 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//��ɂԂ������͉̂����c��-����̉���-
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

                if (motion_processing.scriptcol_y.x < befor_scriptcol_y.y && motion_processing.scriptcol_y.x >= scriptcol_y.y && motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_x.x > scriptcol_x.y)//��ɂԂ������͉̂����c��-����̏��-
                {
                    set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.x, false));
                    square_ground_wall_down[count - 1] = true;
                }
            }

            set_cols();

            if (square_ground_wall_right[count - 1] == false && scriptcol_x.x == motion_processing.scriptcol_x.y && motion_processing.scriptcol_y.x > scriptcol_y.y && motion_processing.scriptcol_y.y < scriptcol_y.x)//�E�Ԃ���
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                square_ground_wall_right[count - 1] = true;
                Debug.Log("hello-");
            }
            if (square_ground_wall_left[count - 1] == false && scriptcol_x.y == motion_processing.scriptcol_x.x && motion_processing.scriptcol_y.x > scriptcol_y.y && motion_processing.scriptcol_y.y < scriptcol_y.x)//���Ԃ���
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                square_ground_wall_left[count - 1] = true;
                //Debug.Log("hello-");
            }
            if (square_ground_wall_up[count - 1] == false && scriptcol_y.x == motion_processing.scriptcol_y.y && motion_processing.scriptcol_x.x > scriptcol_x.y && motion_processing.scriptcol_x.y < scriptcol_x.x)
            {
                square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                square_ground_wall_up[count - 1] = true;
            }
            if (square_ground_wall_down[count - 1] == false && scriptcol_y.y == motion_processing.scriptcol_y.x && motion_processing.scriptcol_x.x > scriptcol_x.y && motion_processing.scriptcol_x.y < scriptcol_x.x)
            {
                square_ground_wall_down[count - 1] = true;
            }

            count -= 1;
        }

        insertposition();

        {
            int true_ = 0;

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_right, true))
            {//�E��

                float distance = Mathf.Min(square_ground_wall_right_distance);
                int count_;
                Debug.Log(square_ground_wall_right_distance + "" + distance);
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_right_distance, distance);
                    //try
                    //{
                    set_dummy_transform_position(true, change_col_to_pos_x(grounds[count_].GetComponent<motion>().scriptcol_x.y), false, 0);
                    touch_right = true;

                    if (0 < movementvalue.x)
                    {
                        movementvalue.x = 0;
                    }
                    //}
                    //catch (IndexOutOfRangeException)
                    //{
                    //    //Doing nothing
                    //}
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_right, true))
            {
                touch_right = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_left, true))
            {//����

                float distance = Mathf.Min(square_ground_wall_left_distance);
                int count_;
                Debug.Log(square_ground_wall_left_distance + "" + distance);
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_left_distance, distance);
                    //try
                    //{
                    set_dummy_transform_position(true, change_col_to_pos_x(grounds[count_].GetComponent<motion>().scriptcol_x.x, false), false, 0);
                    touch_left = true;

                    if (movementvalue.x < 0)
                    {
                        movementvalue.x = 0;
                    }
                    //}
                    //catch (IndexOutOfRangeException)
                    //{
                    //    //Doing nothing
                    //}
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_left, true))
            {
                touch_left = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (grounds.Length != 0 && ArrayUtility.Contains<bool>(square_ground_wall_up, true))
            {//�㑤

                float distance = Mathf.Min(square_ground_wall_up_distance);
                int count_;
                Debug.Log(square_ground_wall_up_distance + "" + distance);
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_up_distance, distance);
                    //try
                    //{
                    set_dummy_transform_position(false, 0, true, change_col_to_pos_y(grounds[count_].GetComponent<motion>().scriptcol_y.y));
                    touch_up = true;

                    if (0 < movementvalue.y)
                    {
                        movementvalue.y = 0;
                    }
                    //}
                    //catch (IndexOutOfRangeException)
                    //{
                    //    //Doing nothing
                    //}
                }
            }

            if (grounds.Length != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_up, true))
            {
                touch_up = false;
            }
            //for (int count_ = square_ground_wall_left.Length; count_ > 0; --count_)//�I�u�W�F�N�g����
            //{
            //    //Debug.Log(count + square_ground_wall_left[count - 1].ToString());
            //    if (square_ground_wall_left[count_ - 1] == true)
            //    {
            //        true_ += 1;
            //    }

            //    if (true_ > 0)
            //    {
            //        touch_left = true;
            //        if (movementvalue.x < 0)
            //        {
            //            //dummy_transform_position.x -= movementvalue.x;
            //            //Debug.Log("hhh");
            //            movementvalue.x = 0;
            //        }
            //        //Debug.Log("touched");
            //    }
            //    else touch_left = false;
            //}

            true_ = 0;

            //for (int count_ = square_ground_wall_right.Length; count_ > 0; --count_)//�I�u�W�F�N�g�E��
            //{
            //    if (square_ground_wall_right[count_ - 1] == true)
            //    {
            //        true_ += 1;
            //    }

            //    if (true_ > 0)
            //    {
            //        touch_right = true;
            //        if (movementvalue.x > 0)
            //        {
            //            //dummy_transform_position.x -= movementvalue.x;
            //            movementvalue.x = 0;
            //        }
                    
            //    }
            //    else touch_right = false;
            //}

            true_ = 0;

            //for (int count_ = square_ground_wall_up.Length; count_ > 0; --count_)//�I�u�W�F�N�g�㑤
            //{
            //    if (square_ground_wall_up[count_ - 1] == true)
            //    {
            //        true_ += 1;
            //    }

            //    if (true_ > 0)
            //    {
            //        touch_up = true;
            //        movementvalue.y = 0;
            //    }
            //    else touch_up = false;
            //}

            true_ = 0;

            for (int count_ = square_ground_wall_down.Length; count_ > 0; --count_)//�I�u�W�F�N�g����
            {
                if (square_ground_wall_down[count_ - 1] == true)
                {
                    true_ += 1;
                }

                if (true_ > 0)
                {
                    touch_down = true;
                    movementvalue.x = 0;
                }
                else touch_down = false;
            }

            if (touch_down || touch_left || touch_right || touch_up) touching_something = true;
            else touching_something = false;
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

            if (movementvalue.x == 0.3f) Debug.Log("go right");
        }

        if (touch_left && movementvalue.x < 0) movementvalue.x = 0;
        if (touch_right && movementvalue.x > 0) movementvalue.x = 0;
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
        if (is_x) return colx - ((box2d.size.x * transform.localScale.x) / 2);
        else return colx - ((-box2d.size.x * transform.localScale.x) / 2);
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
    }

    void drowline(Vector3 start, Vector3 end)
    {
        Debug.DrawLine(start, end, Color.red, 2,false);
    }
}
