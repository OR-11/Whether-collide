using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class motion : MonoBehaviour
{
    public bool debuging;

    public bool ground;

    public float Air_resistance;
    public float gravity;

    public Vector2 scriptcol_x;
    public Vector2 scriptcol_y;

    public Vector2 befor_scriptcol_x;
    public Vector2 befor_scriptcol_y;

    public Vector2 movementvalue;

    public Vector3 dummy_transform_position;
    public Vector3 befor_transform_position;

    BoxCollider2D box2d;

    motion script_motion;

    public GameObject[] grounds = new GameObject[0];

    public bool[] square_ground_wall_right;

    public bool[] square_ground_wall_left;

    public bool[] square_ground_wall_up;

    public bool[] square_ground_wall_down;

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

        if (ground == false) collision();
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
        ////b3：左下の点のb

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
            is_x_0 = true;
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
            is_y_0 = true;
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

            is_y_0 = false;
        }

        motion motion_processing;

        while (count > 0)
        {


            //motion_processing = null;
            motion_processing = grounds[count - 1].GetComponent<motion>();

            //if (dummy_transform_position.x == -6.72f)
            //{
            //    Debug.Log("-6.72f");
            //}

            //if (dummy_transform_position.x < -6.72f)
            //{
            //    Debug.Log("-6.72fより大きい");
            //}

            {//動いていないときの判定
                if (dummy_transform_position.x == befor_transform_position.x)
                {
                    if (square_ground_wall_right[count - 1] == false && scriptcol_x.x >= motion_processing.scriptcol_x.y && scriptcol_x.y <= motion_processing.scriptcol_x.x && dummy_transform_position.x < motion_processing.dummy_transform_position.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//右側
                    {
                        dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                        square_ground_wall_right[count - 1] = true;
                    }
                    else square_ground_wall_right[count - 1] = false;

                    if (square_ground_wall_left[count - 1] == false && scriptcol_x.y <= motion_processing.scriptcol_x.x && scriptcol_x.x >= motion_processing.scriptcol_x.y && dummy_transform_position.x > motion_processing.dummy_transform_position.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//左側
                    {
                        dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                        square_ground_wall_left[count - 1] = true;
                        Debug.Log("working");//問題なし
                    }
                    else
                    {
                        square_ground_wall_left[count - 1] = false;
                        //Debug.Log("falsed");
                    }
                    Debug.Log("no move");//右押すとここが動かない(移動してないのに)
                }
                //else Debug.Log(dummy_transform_position + "," + befor_transform_position);
            }


            {//動いてないとき用
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_right[count - 1] && (scriptcol_x.x != motion_processing.scriptcol_x.y || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.y))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//右側ぶつかる
                {
                    square_ground_wall_right[count - 1] = false;
                }
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_left[count - 1] && (scriptcol_x.y != motion_processing.scriptcol_x.x || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.x))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//左側ぶつかる
                {
                    square_ground_wall_left[count - 1] = false;
                    Debug.Log("c");
                }
            }

            //player_pos();

            if (square_ground_wall_right[count - 1] && square_ground_wall_up[count - 1] == false && square_ground_wall_down[count - 1] == false && motion_processing.scriptcol_x.y < scriptcol_x.x)//右側
            {
                dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
            }
            if (square_ground_wall_left[count - 1] && square_ground_wall_up[count - 1] == false && square_ground_wall_down[count - 1] == false && motion_processing.scriptcol_x.x > scriptcol_x.y)//左側
            {
                dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
            }
            //なにかあった?(解決済み...?)

            


            if (is_y_0 == true && is_x_0 == false)//X座標のみ
            {
                //右でも左でも

                if (scriptcol_x.x >= motion_processing.scriptcol_x.y && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && touch_left == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//右側ぶつかる
                {
                    dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                    square_ground_wall_right[count - 1] = true;
                    Debug.Log("touch-right" + touch_left);
                }
                else
                {
                    square_ground_wall_right[count - 1] = false;
                    Debug.Log("falsed" + debugcount);
                    Debug.Log((scriptcol_x.x >= motion_processing.scriptcol_x.y) + "" + (befor_scriptcol_x.x <= motion_processing.scriptcol_x.y) + (touch_left == false) + (scriptcol_y.x > motion_processing.scriptcol_y.y) + (scriptcol_y.y < motion_processing.scriptcol_y.x));
                    debugcount += 1;
                }

                if (scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)//左側ぶつかる
                {
                    dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                    square_ground_wall_left[count - 1] = true;
                    times_touching_left = 0;
                    Debug.Log("a");
                }
                else
                {
                    square_ground_wall_left[count - 1] = false;
                    //Debug.Log("falsed");
                }

                //touching_something = true;
            }
            else if (is_x_0 == false)
            {
                touching_something = true;

                if ((a1 * motion_processing.scriptcol_x.y + b1) >= motion_processing.scriptcol_y.y && (a1 * motion_processing.scriptcol_x.y + b1) <= motion_processing.scriptcol_y.x && touch_left == false)//右側ぶつかる
                {
                    dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                    square_ground_wall_right[count - 1] = true;
                }

                if ((a1 * motion_processing.scriptcol_x.x + b1) >= motion_processing.scriptcol_y.y && (a1 * motion_processing.scriptcol_x.x + b1) <= motion_processing.scriptcol_y.x && touch_right == false)//左側ぶつかる
                {
                    dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                    square_ground_wall_left[count - 1] = true;
                    //Debug.Log("working1");
                }
            }

            set_cols();
            //Debug.Log(square_ground_wall_left[count - 1]);

            if (square_ground_wall_right[count - 1] == false && scriptcol_x.x == motion_processing.scriptcol_x.y)
            {
                dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                square_ground_wall_right[count - 1] = true;
                //Debug.Log("hello-");
            }
            if (square_ground_wall_left[count - 1] == false && scriptcol_x.y == motion_processing.scriptcol_x.x)
            {
                dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                square_ground_wall_left[count - 1] = true;
                Debug.Log("hello-");//ここが動いてる、今度はなんでfalseが検知されちゃうか(ここが実行される前になんで動かない?,,,コリダーリセットが必要 => 関数で再設定 - 関数追加済み、あとはいい感じに関数呼び出しを配置)
            }
            if (square_ground_wall_up[count - 1] == false && scriptcol_y.x == motion_processing.scriptcol_y.y) square_ground_wall_up[count - 1] = true;

            if (square_ground_wall_down[count - 1] == false && scriptcol_y.y == motion_processing.scriptcol_y.x) square_ground_wall_down[count - 1] = true;

            //使うとおかしくなる。というか使わなくてもうまく位置が調整されない　調整して進んだのが動いたのが描画されている?
            //if (square_ground_wall_left[count - 1] || square_ground_wall_right[count - 1]) dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);

            count -= 1;
        }

        insertposition();

        {
            int true_ = 0;

            for (int count_ = square_ground_wall_left.Length; count_ > 0; --count_)//オブジェクト左側
            {
                //Debug.Log(count + square_ground_wall_left[count - 1].ToString());
                if (square_ground_wall_left[count_ - 1] == true)
                {
                    true_ += 1;
                }

                if (true_ > 0)
                {
                    touch_left = true;
                    if (movementvalue.x < 0)
                    {
                        dummy_transform_position.x -= movementvalue.x;
                        movementvalue.x = 0;
                    }
                    //Debug.Log("touched");
                }
                else touch_left = false;
            }

            true_ = 0;

            for (int count_ = square_ground_wall_right.Length; count_ > 0; --count_)//オブジェクト右側
            {
                if (square_ground_wall_right[count_ - 1] == true)
                {
                    true_ += 1;
                }

                if (true_ > 0)
                {
                    touch_right = true;
                }
                else touch_right = false;
            }

            true_ = 0;

            for (int count_ = square_ground_wall_up.Length; count_ > 0; --count_)//オブジェクト上側
            {
                if (square_ground_wall_up[count_ - 1] == true)
                {
                    true_ += 1;
                }

                if (true_ > 0)
                {
                    touch_up = true;
                }
                else touch_up = false;
            }

            true_ = 0;

            for (int count_ = square_ground_wall_down.Length; count_ > 0; --count_)//オブジェクト下側
            {
                if (square_ground_wall_down[count_ - 1] == true)
                {
                    true_ += 1;
                }

                if (true_ > 0)
                {
                    touch_down = true;
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
            is_x_0 = true;
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
            is_y_0 = true;
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

            is_y_0 = false;
        }
    }

    void colcount()
    {
        square_ground_wall_down = new bool[grounds.Length];

        square_ground_wall_up = new bool[grounds.Length];

        square_ground_wall_right = new bool[grounds.Length];

        square_ground_wall_left = new bool[grounds.Length];
    }

    void drowline(Vector3 start, Vector3 end)
    {
        Debug.DrawLine(start, end, Color.red, 2,false);
    }
}
