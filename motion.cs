using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System;
using System.Runtime.CompilerServices;

public class motion : MonoBehaviour
{
    [System.Serializable]
    public class Debugs
    {
        public bool debuging;
        public bool collideLine_type_P;
        public bool collideLine_type_ScriptCol;
        public bool collideLine_type_BeforeScriptCol;
        public bool movementDirection;
    }
    public Debugs debugs;

    [System.Serializable]
    public class FunctionSetting
    {
        public enum UsingFunction //選択肢
        {
            Update,
            FixedUpdate,
            None,
        }

        public UsingFunction usingFunction = UsingFunction.FixedUpdate;//選択肢の宣言
    }

    public FunctionSetting FunctionSettings;

    class PositionRelAbs
    {
        public PositionRelAbs(float x, float y, bool absX = false, bool absY = false)
        {
            pos = new Vector2(x, y);
            this.absX = absX;
            this.absY = absY;
        }
        public Vector2 pos;
        public bool absX;
        public bool absY;
    }

    public class CollisionLine//1次関数
    {
        public CollisionLine(float a, float b, bool NotMove = false, bool xEqual0 = false, bool yEqual0 = false)//y = ax + b
        {
            this.a = a;
            this.b = b;
            this.NotMove = NotMove;
            this.xEqual0 = xEqual0;
            this.yEqual0 = yEqual0;
        }
        public float a;
        public float b;
        public bool xEqual0;
        public bool yEqual0;
        public bool NotMove;
    }

    //public class bool3
    //{
    //    public bool3(bool x, bool y, bool z)
    //    {
    //        this.x = x;
    //        this.y = y;
    //        this.z = z;
    //    }
    //    public bool x;
    //    public bool y;
    //    public bool z;
    //}

    public class dummy_transform_position_to_set_container
    {
        public dummy_transform_position_to_set_container(Vector3 Values, bool SetBeforeCol)
        {
            //this.IsAbsolute = IsAbsolute;
            this.Values = Values;
            this.SetBeforeCol = SetBeforeCol;
        }
        //public bool3 IsAbsolute;
        public Vector3 Values;
        public bool SetBeforeCol;
    }
    public List<dummy_transform_position_to_set_container> dummy_transform_position_to_set_container_List = new List<dummy_transform_position_to_set_container>();

    [System.Serializable]
    public class ObjectSetting
    {
        public bool ground;
        [HideInInspector]
        public bool only_whether_touch;
        [HideInInspector]
        public bool stop_when_this_collide;
        [HideInInspector]
        public int priority;
        [HideInInspector]
        public float Air_resistance;
        public float gravity;
        public bool useGravity;

        //[HideInInspector]//スクリプト間情報
        //public bool Rotate;
        //[HideInInspector]
        //public bool IsTrigger;

        public class ObjectCondition
        {
            public bool Rotate;
            public bool IsTrigger;
            //public bool Changed;
        }
        public ObjectCondition Condition;
    }
    public ObjectSetting ObjectSettings;

    //
    //c1 右上
    //c2 左上
    //c3 右下
    //c4 左下
    public CollisionLine c1;
    public CollisionLine c2;
    public CollisionLine c3;
    public CollisionLine c4;

    public CollisionLine B_c1;
    public CollisionLine B_c2;
    public CollisionLine B_c3;
    public CollisionLine B_c4;

    //------------------------
    public CollisionLine c1_B_c1;
    public CollisionLine c2_B_c2;
    public CollisionLine c3_B_c3;
    public CollisionLine c4_B_c4;
    //------------------------

    public Vector2 LocalScriptCol_X;
    public Vector2 LocalScriptCol_Y;

    public Vector2 LocalScriptCol_X_IncludeRotate;
    public Vector2 LocalScriptCol_Y_IncludeRotate;

    //---------------------------
    //回転したとき用(絶対座標)
    //p1:右上
    //p2:左上
    //p3:右下
    //p4:左下
    public Vector2 p1;
    public Vector2 p2;
    public Vector2 p3;
    public Vector2 p4;

    //相対的
    //Rp1:右上
    //Rp2:左上
    //Rp3:右下
    //Rp4:左下
    public Vector2 Rp1;
    public Vector2 Rp2;
    public Vector2 Rp3;
    public Vector2 Rp4;
    //---------------------------
    //回転したとき用(絶対座標)、1フレーム前
    //p1:右上
    //p2:左上
    //p3:右下
    //p4:左下
    public Vector2 B_p1;
    public Vector2 B_p2;
    public Vector2 B_p3;
    public Vector2 B_p4;
    //---------------------------

    //元の計算用(代わりに置き換え)
    public (float X_BiggerOne, float X_SmallerOne, float Y_BiggerOne, float Y_SmallerOne) OriginalScriptCol;

    public int ToSetStartFrom = 0;

    //計算用(置き換えめんどいから流用)
    public Vector2 scriptcol_x;
    public Vector2 scriptcol_y;

    public Vector2 befor_scriptcol_x;
    public Vector2 befor_scriptcol_y;

    public Vector2 movementvalue;

    Vector2 mysize;

    float OwnRotate;

    //PositionRelAbs[] candidate = new PositionRelAbs[4];

    //bool[] absolutelyX = new bool[4];
    //bool[] absolutelyY = new bool[4];

    //Vector2[] comparison = new Vector2[4];
    //float[] comparisonX = new float[4];
    //float[] comparisonY = new float[4];
    //Vector2 IsPositive;

    //Vector2 ResultOfSetDummyTransformPosition;

    //int SetXCount;
    //int SetYCount;

    public Vector3 dummy_transform_position;
    public Vector3 befor_transform_position;

    public Vector3 dummy_transform_position_to_set;
    public int set_befor_col;
    public bool do_dummy_transform_position_to_set_execute;
    int calledCount;

    BoxCollider2D box2d;

    motion script_motion;

    //public GameObject[] HideGrounds = new GameObject[0];
    //public int num;

    //GameObject[] objects = new GameObject[0];
    //public List<GameObject> objects = new List<GameObject>();

    public List<GameObject> objects = new List<GameObject>();

    public bool[] square_ground_wall_right;

    public bool[] square_ground_wall_left;

    public bool[] square_ground_wall_up;

    public bool[] square_ground_wall_down;

    //----------------------------------------------------------------------
    //座標の差(右/左:X座標 | 上/下:Y座標)の絶対値を格納する
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

    bool before_touching_something;

    public GameObject[] touching_right;

    public GameObject[] touching_left;

    public GameObject[] touching_up;

    public GameObject[] touching_down;

    public GameObject[] touching;

    public int touchingCount;

    //自作コリダーたち
    //変数準備
    int count;// = objects.Count;
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

    void Awake()
    {
        ObjectSettings.Condition = new ObjectSetting.ObjectCondition();
    }

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
        if (debugs.debuging == true)
        {
            if (debugs.collideLine_type_P)
            {
                drowline(new Vector3(p1.x, p1.y, 0), new Vector3(p2.x, p2.y, 0), Color.red, 0);

                drowline(new Vector3(p3.x, p3.y, 0), new Vector3(p4.x, p4.y, 0), Color.red, 0);

                drowline(new Vector3(p1.x, p1.y, 0), new Vector3(p3.x, p3.y, 0), Color.red, 0);

                drowline(new Vector3(p2.x, p2.y, 0), new Vector3(p4.x, p4.y, 0), Color.red, 0);
            }

            if (debugs.collideLine_type_ScriptCol)
            {
                drowline(new Vector3(scriptcol_x.x, scriptcol_y.x, 0), new Vector3(scriptcol_x.x, scriptcol_y.y, 0), Color.red, 0);

                drowline(new Vector3(scriptcol_x.x, scriptcol_y.x, 0), new Vector3(scriptcol_x.y, scriptcol_y.x, 0), Color.red, 0);

                drowline(new Vector3(scriptcol_x.y, scriptcol_y.x, 0), new Vector3(scriptcol_x.y, scriptcol_y.y, 0), Color.red, 0);

                drowline(new Vector3(scriptcol_x.x, scriptcol_y.y, 0), new Vector3(scriptcol_x.y, scriptcol_y.y, 0), Color.red, 0);
            }

            if (debugs.collideLine_type_BeforeScriptCol)
            {
                drowline(new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.y, 0), Color.red, 0);

                drowline(new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.x, 0), Color.red, 0);

                drowline(new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.y, 0), Color.red, 0);

                drowline(new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.y, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.y, 0), Color.red, 0);
            }
            //scriptcol_x = new Vector2((dummy_transform_position.x + (box2d.size.x * (transform.localScale.x * 0.5f))), (dummy_transform_position.x + (-box2d.size.x * (transform.localScale.x * 0.5f))));
            //scriptcol_y = new Vector2((dummy_transform_position.y + (box2d.size.y * (transform.localScale.y * 0.5f))), (dummy_transform_position.y + (-box2d.size.y * (transform.localScale.y * 0.5f))));

            //Debug.Log("x:" + scriptcol_x + "y:" + scriptcol_y);

            //drowline(new Vector3(scriptcol_x.x, scriptcol_y.x, -1), new Vector3(scriptcol_x.y, scriptcol_y.x, -1));

            if (debugs.movementDirection) drowline(dummy_transform_position, befor_transform_position, Color.red, 0);
            //drowline(new Vector3(scriptcol_x.x, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.x, 0));
            //drowline(new Vector3(scriptcol_x.y, befor_scriptcol_y.x, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.x, 0));
            //drowline(new Vector3(scriptcol_x.x, befor_scriptcol_y.y, 0), new Vector3(befor_scriptcol_x.x, befor_scriptcol_y.y, 0));
            //drowline(new Vector3(scriptcol_x.y, befor_scriptcol_y.y, 0), new Vector3(befor_scriptcol_x.y, befor_scriptcol_y.y, 0));

            //drowline(new Vector3(dummy_transform_position.x, LocalScriptCol_Y.x + dummy_transform_position.y, 0), new Vector3(dummy_transform_position.x, LocalScriptCol_Y.y + dummy_transform_position.y, 0));

            //if (Input.GetKey(KeyCode.Space)) movement(new Vector2(-1, 0), true);
            //else movement(new Vector2(0, 0), false);
            //if (Input.GetKey(KeyCode.P)) Debug.Log(scriptcol_x);
            //if (Input.GetKey(KeyCode.D)) Debug.Log(Mathf.Approximately())
        }

        if (FunctionSettings.usingFunction == FunctionSetting.UsingFunction.Update) Main();
    }

    private void FixedUpdate()
    {
        if (FunctionSettings.usingFunction == FunctionSetting.UsingFunction.FixedUpdate) Main();
    }

    public void Main([CallerMemberName] string CalledFrom = "")
    {
        ToSetStartFrom = 0;

        //Null一掃
        objects.RemoveAll(elementIsNull);

        OwnRotate = transform.localRotation.eulerAngles.z;
        if (Mathf.Approximately(OwnRotate, 0)) OwnRotate = 0;
        else if (Mathf.Approximately(OwnRotate, 90)) OwnRotate = 90;
        else if (Mathf.Approximately(OwnRotate, 180)) OwnRotate = 180;
        else if (Mathf.Approximately(OwnRotate, 360)) OwnRotate = 360;

        dummy_transform_position = transform.position;
        ObjectSettings.Condition.IsTrigger = box2d.isTrigger;
        ObjectSettings.Condition.Rotate = (OwnRotate % 90 == 0);

        //if (dummy_transform_position != befor_transform_position) Debug.Log("not same");

        Movement(new Vector2(0, 0), false);


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

        if (CalledFrom != "MoveFromSet" && set_befor_col > 0 && do_dummy_transform_position_to_set_execute)
        {
            while (RangeCheck<dummy_transform_position_to_set_container>(dummy_transform_position_to_set_container_List, ToSetStartFrom) && dummy_transform_position_to_set_container_List[ToSetStartFrom].SetBeforeCol)
            {
                dummy_transform_position_to_set_container_List[ToSetStartFrom].Values = check_change_dummy_transform_position(dummy_transform_position_to_set_container_List[ToSetStartFrom].Values);
                dummy_transform_position = dummy_transform_position_to_set_container_List[ToSetStartFrom].Values;
                ToSetStartFrom++;
            }

            //check_change_dummy_transform_position(dummy_transform_position_to_set);
            if (ToSetStartFrom == dummy_transform_position_to_set_container_List.Count - 1) do_dummy_transform_position_to_set_execute = false;
            //dummy_transform_position = dummy_transform_position_to_set;
        }



        LocalScriptCol_X = new Vector2((box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x), (-box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x));
        LocalScriptCol_Y = new Vector2((box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y), (-box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y));

        //scriptcol_x = new Vector2((dummy_transform_position.x + LocalScriptCol_X.x), (dummy_transform_position.x + LocalScriptCol_X.y));//(box2d.size.x * (transform.localScale.x * 0.5f)),(-box2d.size.x * (transform.localScale.x * 0.5f))
        //scriptcol_y = new Vector2((dummy_transform_position.y + LocalScriptCol_Y.x), (dummy_transform_position.y + LocalScriptCol_Y.y));//(box2d.size.y * (transform.localScale.y * 0.5f)),(-box2d.size.y * (transform.localScale.y * 0.5f))

        //--------------------------------------------------------------------------------------たぶんいらない
        OriginalScriptCol.X_BiggerOne = dummy_transform_position.x + LocalScriptCol_X.x;
        OriginalScriptCol.X_SmallerOne = dummy_transform_position.x + LocalScriptCol_X.y;

        OriginalScriptCol.Y_BiggerOne = dummy_transform_position.y + LocalScriptCol_Y.x;
        OriginalScriptCol.Y_SmallerOne = dummy_transform_position.y + LocalScriptCol_Y.y;
        //--------------------------------------------------------------------------------------
        //メモ
        {
            //回転したとき用
            //p1:右上
            //p2:左上
            //p3:右下
            //p4:左下
        }
        //--------------------------------------------------------------------------------------
        //回転したとき用
        //p1:右上
        //p2:左上
        //p3:右下
        //p4:左下
        //Debug.Log("" + Mathf.Atan(LocalScriptCol_Y.x / LocalScriptCol_X.x) + "__" + Mathf.Atan(LocalScriptCol_Y.x / LocalScriptCol_X.y));

        //引継ぎ
        B_p1 = p1;
        B_p2 = p2;
        B_p3 = p3;
        B_p4 = p4;

        float p1Atan = Mathf.Atan(LocalScriptCol_Y.x / LocalScriptCol_X.x);
        float ra = (Mathf.PI * -transform.localRotation.eulerAngles.z) / 180;
        float p1sqrt = Mathf.Sqrt(Mathf.Pow(LocalScriptCol_X.x, 2) + Mathf.Pow(LocalScriptCol_Y.x, 2));

        float p2Atan = Mathf.Atan(LocalScriptCol_Y.x / LocalScriptCol_X.y) - Mathf.PI;
        float p2sqrt = Mathf.Sqrt(Mathf.Pow(LocalScriptCol_X.y, 2) + Mathf.Pow(LocalScriptCol_Y.x, 2));

        float p3Atan = Mathf.Atan(LocalScriptCol_Y.y / LocalScriptCol_X.x);
        float p3sqrt = Mathf.Sqrt(Mathf.Pow(LocalScriptCol_X.x, 2) + Mathf.Pow(LocalScriptCol_Y.y, 2));

        float p4Atan = Mathf.Atan(LocalScriptCol_Y.y / LocalScriptCol_X.y) - Mathf.PI;
        float p4sqrt = Mathf.Sqrt(Mathf.Pow(LocalScriptCol_X.y, 2) + Mathf.Pow(LocalScriptCol_Y.y, 2));

        //Debug.Log(name + p1Atan / Mathf.PI * 180);

        p1 = new Vector2(dummy_transform_position.x + (Mathf.Cos((p1Atan - ra)) * p1sqrt), dummy_transform_position.y + (Mathf.Sin((p1Atan - ra)) * p1sqrt));
        p2 = new Vector2(dummy_transform_position.x + (Mathf.Cos((p2Atan - ra)) * p2sqrt), dummy_transform_position.y + (Mathf.Sin((p2Atan - ra)) * p2sqrt));
        //p1 = new Vector2(dummy_transform_position.x + (Mathf.Cos(((transform.rotation.z * Mathf.PI)) + (LocalScriptCol_Y.x / Mathf.Tan(LocalScriptCol_X.x))) * Mathf.Sqrt(Mathf.Pow(LocalScriptCol_X.x, 2) + Mathf.Pow(LocalScriptCol_Y.x, 2))), dummy_transform_position.y + (Mathf.Sin(((transform.rotation.z * Mathf.PI)/180f) + (LocalScriptCol_Y.x / Mathf.Tan(LocalScriptCol_X.x))) * Mathf.Sqrt(Mathf.Pow(LocalScriptCol_X.x, 2) + Mathf.Pow(LocalScriptCol_Y.x, 2))));
        //p2 = new Vector2((-dummy_transform_position.x * 2) + (Mathf.Cos((transform.rotation.z * Mathf.PI) + (LocalScriptCol_Y.x / Mathf.Tan(-LocalScriptCol_X.y))) * Mathf.Sqrt(Mathf.Pow(-LocalScriptCol_X.y, 2) + Mathf.Pow(LocalScriptCol_Y.x, 2))), dummy_transform_position.y + (Mathf.Sin((transform.rotation.z * Mathf.PI) + (LocalScriptCol_Y.x / Mathf.Tan(-LocalScriptCol_X.y))) * Mathf.Sqrt(Mathf.Pow(-LocalScriptCol_X.y, 2) + Mathf.Pow(LocalScriptCol_Y.x, 2))));
        p3 = new Vector2(dummy_transform_position.x + (Mathf.Cos((p3Atan - ra)) * p3sqrt), dummy_transform_position.y + (Mathf.Sin((p3Atan - ra)) * p3sqrt));
        p4 = new Vector2(dummy_transform_position.x + (Mathf.Cos((p4Atan - ra)) * p4sqrt), dummy_transform_position.y + (Mathf.Sin((p4Atan - ra)) * p4sqrt));
        mysize = transform.localScale;

        switch (OwnRotate)
        {
            case 0:
                scriptcol_x = new Vector2(p1.x, p2.x);//(OriginalScriptCol.X_BiggerOne, OriginalScriptCol.X_SmallerOne);
                scriptcol_y = new Vector2(p1.y, p3.y);//(OriginalScriptCol.Y_BiggerOne, OriginalScriptCol.Y_SmallerOne);

                LocalScriptCol_X_IncludeRotate = LocalScriptCol_X;
                LocalScriptCol_Y_IncludeRotate = LocalScriptCol_Y;
                break;

            case 90:
                scriptcol_x = new Vector2(p3.x, p1.x);//(OriginalScriptCol.Y_SmallerOne, OriginalScriptCol.Y_BiggerOne);
                scriptcol_y = new Vector2(p1.y, p2.y);//(OriginalScriptCol.X_BiggerOne, OriginalScriptCol.X_SmallerOne);

                LocalScriptCol_X_IncludeRotate = new Vector2((box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y), (-box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y));
                LocalScriptCol_Y_IncludeRotate = new Vector2((box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x), (-box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x));
                break;

            case 180:
                scriptcol_x = new Vector2(p2.x, p1.x);
                scriptcol_y = new Vector2(p3.y, p1.y);

                LocalScriptCol_X_IncludeRotate = new Vector2(-LocalScriptCol_X.y, -LocalScriptCol_X.x);
                LocalScriptCol_Y_IncludeRotate = new Vector2(-LocalScriptCol_Y.y, -LocalScriptCol_Y.x);
                break;

            case 270:
                scriptcol_x = new Vector2(p1.x, p3.x);
                scriptcol_y = new Vector2(p2.y, p1.y);

                LocalScriptCol_X_IncludeRotate = new Vector2(-new Vector2((box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y), (-box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y)).y, -new Vector2((box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y), (-box2d.size.y * (transform.localScale.y * 0.5f)) + (box2d.offset.y * transform.localScale.y)).x);
                LocalScriptCol_Y_IncludeRotate = new Vector2(-new Vector2((box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x), (-box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x)).y, -new Vector2((box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x), (-box2d.size.x * (transform.localScale.x * 0.5f)) + (box2d.offset.x * transform.localScale.x)).x);
                break;
        }

        if (OwnRotate > 0 && OwnRotate < 90)
        {
            Rp1 = p1;
            Rp2 = p2;
            Rp3 = p3;
            Rp4 = p4;
        }
        else if (OwnRotate > 90 && OwnRotate < 180)
        {
            Rp1 = p3;
            Rp2 = p1;
            Rp3 = p4;
            Rp4 = p2;
        }
        else if (OwnRotate > 180 && OwnRotate < 270)
        {
            Rp1 = p4;
            Rp2 = p3;
            Rp3 = p2;
            Rp4 = p1;
        }
        else if (OwnRotate > 270)
        {
            Rp1 = p2;
            Rp2 = p4;
            Rp3 = p1;
            Rp4 = p3;
        }

        if (ObjectSettings.Condition.Rotate)
        {
            collisionNoRotate();
        }
        else if (ObjectSettings.ground == false) collisionRotate();

        if (CalledFrom != "MoveFromSet" && calledCount != set_befor_col && do_dummy_transform_position_to_set_execute)
        {
            //for (int i = start; i < dummy___container_List.Length && dummy___container_List[i].SetBeforeCol; i++)
            //{

            //}
            while (RangeCheck<dummy_transform_position_to_set_container>(dummy_transform_position_to_set_container_List, ToSetStartFrom) && !dummy_transform_position_to_set_container_List[ToSetStartFrom].SetBeforeCol)
            {
                dummy_transform_position = dummy_transform_position_to_set_container_List[ToSetStartFrom].Values;
                ToSetStartFrom++;
            }
            if (ToSetStartFrom == dummy_transform_position_to_set_container_List.Count - 1) do_dummy_transform_position_to_set_execute = false;
            //dummy_transform_position = dummy_transform_position_to_set;
        }
        insertposition();

        if (Input.GetKey(KeyCode.Space) && debugs.debuging)
        {
            Debug.Log(touch_down);
        }

        //HideGrounds = objects;
        if (ToSetStartFrom != dummy_transform_position_to_set_container_List.Count - 1) MoveFromSet(ToSetStartFrom);
        dummy_transform_position_to_set_container_List.Clear();
        calledCount = 0;
    }

    void collisionRotate()
    {
        //R
        //p1:右上の点
        //p2:左上の点
        //p3:右下の点
        //p4:左下の点

        //a1:右上の辺の比例定数
        //a2:左上の辺の比例定数
        //a3:右下の辺の比例定数
        //a4:左下の辺の比例定数

        //b1：右上の辺のb
        //b2：左上の辺のb
        //b3：右下の辺のb
        //b4：左下の辺のb

        //いろいろ準備
        count = objects.Count;
        motion motion_processing;
        //Vector2 motion_TransSize;

        B_c1 = c1;
        B_c2 = c2;
        B_c3 = c3;
        B_c4 = c4;

        //
        //c1 右上
        //c2 左上
        //c3 右下
        //c4 左下
        c1 = new CollisionLine((Rp3.y - Rp1.y) / (Rp3.x - Rp1.x), dummy_transform_position.y - ((Rp3.y - Rp1.y) / (Rp3.x - Rp1.x)) * dummy_transform_position.x);
        c2 = new CollisionLine((Rp1.y - Rp4.y) / (Rp1.x - Rp4.x), dummy_transform_position.y - ((Rp1.y - Rp4.y) / (Rp1.x - Rp4.x)) * dummy_transform_position.x);
        c3 = new CollisionLine((Rp3.y - Rp4.y) / (Rp3.x - Rp4.x), dummy_transform_position.y - ((Rp3.y - Rp4.y) / (Rp3.x - Rp4.x)) * dummy_transform_position.x);
        c4 = new CollisionLine((Rp4.y - Rp2.y) / (Rp4.x - Rp2.x), dummy_transform_position.y - ((Rp4.y - Rp2.y) / (Rp4.x - Rp2.x)) * dummy_transform_position.x);

        //動き
        //c1_B_c1：右上ー前右上
        //c2_B_c2：左上ー前左上
        //c3_B_c3：右下ー前右下
        //c4_B_c4：左下ー前左下
        if (!Mathf.Approximately(p1.x, B_p1.x))//通常
        {
            c1_B_c1 = new CollisionLine((p1.x < B_p1.x ? (B_p1.y - p1.y) / (B_p1.x - p1.x) : (p1.y - B_p1.y) / (p1.x - B_p1.x)),
                dummy_transform_position.y - (p1.x < B_p1.x ? (B_p1.y - p1.y) / (B_p1.x - p1.x) : (p1.y - B_p1.y) / (p1.x - B_p1.x)) * dummy_transform_position.x);
        }
        else if (!Mathf.Approximately(p1.y, B_p1.y) && Mathf.Approximately(p1.x, B_p1.x))//上下移動
        {
            c1_B_c1 = new CollisionLine(0, p1.x, false, false, true);
        }
        else if (!Mathf.Approximately(p1.x, B_p1.x) && Mathf.Approximately(p1.y, B_p1.y))//左右移動
        {
            c1_B_c1 = new CollisionLine(0, p1.y, false, true, false);
        }
        else//動かず
        {
            c1_B_c1 = new CollisionLine(0, 0, true);
        }

        //////////////////////////////////////////////////////////////////////////
        
        if (!Mathf.Approximately(p2.x, B_p2.x))//通常
        {
            c2_B_c2 = new CollisionLine((p2.x < B_p2.x ? (B_p2.y - p2.y) / (B_p2.x - p2.x) : (p2.y - B_p2.y) / (p2.x - B_p2.x)),
            dummy_transform_position.y - (p2.x < B_p2.x ? (B_p2.y - p2.y) / (B_p2.x - p2.x) : (p2.y - B_p2.y) / (p2.x - B_p2.x)) * dummy_transform_position.x);
        }
        else if (!Mathf.Approximately(p2.y, B_p2.y) && Mathf.Approximately(p2.x, B_p2.x))//上下移動
        {
            c2_B_c2 = new CollisionLine(0, p2.x, false, false, true);
        }
        else if (!Mathf.Approximately(p2.x, B_p2.x) && Mathf.Approximately(p2.y, B_p2.y))//左右移動
        {
            c2_B_c2 = new CollisionLine(0, p2.y, false, true, false);
        }
        else//動かず
        {
            c2_B_c2 = new CollisionLine(0, 0, true);
        }

        //////////////////////////////////////////////////////////////////////////

        if (!Mathf.Approximately(p3.x, B_p3.x))//通常
        {
            c3_B_c3 = new CollisionLine((p3.x < B_p3.x ? (B_p3.y - p3.y) / (B_p3.x - p3.x) : (p3.y - B_p3.y) / (p3.x - B_p3.x)),
            dummy_transform_position.y - (p3.x < B_p3.x ? (B_p3.y - p3.y) / (B_p3.x - p3.x) : (p3.y - B_p3.y) / (p3.x - B_p3.x)) * dummy_transform_position.x);
        }
        else if (!Mathf.Approximately(p3.y, B_p3.y) && Mathf.Approximately(p3.x, B_p3.x))//上下移動
        {
            c3_B_c3 = new CollisionLine(0, p3.x, false, false, true);
        }
        else if (!Mathf.Approximately(p3.x, B_p3.x) && Mathf.Approximately(p3.y, B_p3.y))//左右移動
        {
            c3_B_c3 = new CollisionLine(0, p3.y, false, true, false);
        }
        else//動かず
        {
            c3_B_c3 = new CollisionLine(0, 0, true);
        }

        //////////////////////////////////////////////////////////////////////////

        if (!Mathf.Approximately(p4.x, B_p4.x))//通常
        {
            c4_B_c4 = new CollisionLine((p4.x < B_p4.x ? (B_p4.y - p4.y) / (B_p4.x - p4.x) : (p4.y - B_p4.y) / (p4.x - B_p4.x)),
            dummy_transform_position.y - (p4.x < B_p4.x ? (B_p4.y - p4.y) / (B_p4.x - p4.x) : (p4.y - B_p4.y) / (p4.x - B_p4.x)) * dummy_transform_position.x);
        }
        else if (!Mathf.Approximately(p4.y, B_p4.y) && Mathf.Approximately(p4.x, B_p4.x))//上下移動
        {
            c4_B_c4 = new CollisionLine(0, p4.x, false, false, true);
        }
        else if (!Mathf.Approximately(p4.x, B_p4.x) && Mathf.Approximately(p4.y, B_p4.y))//左右移動
        {
            c4_B_c4 = new CollisionLine(0, p4.y, false, true, false);
        }
        else//動かず
        {
            c4_B_c4 = new CollisionLine(0, 0, true);
        }
        

        while (count > 0)
        {
            motion_processing = objects[count - 1].GetComponent<motion>();

            square_ground_wall_left[count - 1] = false;
            square_ground_wall_right[count - 1] = false;
            square_ground_wall_up[count - 1] = false;
            square_ground_wall_down[count - 1] = false;

            //Debug.Log(c1_B_c1.a + "、" + c1_B_c1.b);

            //右上
            if (WhereDoLinesCross(c1_B_c1, motion_processing.c3, p1, B_p1) != null)
            {
                Debug.Log("collided");
            }

            count -= 1;
        }
    }

    void collisionNoRotate()
    {
        count = objects.Count;
        //変数準備
        //int count = objects.Count;
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

        //Debug.Log(is_x_0 + "" + is_y_0);

        motion motion_processing;
        Vector2 motion_TransSize;

        while (count > 0)
        {
            motion_processing = objects[count - 1].GetComponent<motion>();

            motion_TransSize = objects[count - 1].transform.localScale;

            square_ground_wall_left[count - 1] = false;
            square_ground_wall_right[count - 1] = false;
            square_ground_wall_up[count - 1] = false;
            square_ground_wall_down[count - 1] = false;

            {//動いてないとき用
                //左右
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_right[count - 1] && (!Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y) || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.y))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//右側ぶつかる
                {
                    square_ground_wall_right[count - 1] = false;
                }
                if (dummy_transform_position.x == befor_transform_position.x && square_ground_wall_left[count - 1] && (!Mathf.Approximately(scriptcol_x.y, motion_processing.scriptcol_x.x) || scriptcol_y.x < motion_processing.scriptcol_y.y || scriptcol_y.y > motion_processing.scriptcol_y.x))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//左側ぶつかる
                {                                                                                                   //scriptcol_x.y != motion_processing.scriptcol_x.x
                    square_ground_wall_left[count - 1] = false;
                    Debug.Log("1up");
                }

                //上下
                if (dummy_transform_position.y == befor_transform_position.y && square_ground_wall_up[count - 1] && (!Mathf.Approximately(scriptcol_y.x, motion_processing.scriptcol_y.y) || scriptcol_x.x < motion_processing.scriptcol_x.y || scriptcol_x.y > motion_processing.scriptcol_x.x))// && scriptcol_y.y <= motion_processing.scriptcol_y.x))//右側ぶつかる
                {
                    square_ground_wall_up[count - 1] = false;
                }
                if (dummy_transform_position.y == befor_transform_position.y && square_ground_wall_down[count - 1] && (!Mathf.Approximately(scriptcol_y.y, motion_processing.scriptcol_y.x) && (scriptcol_x.x < motion_processing.scriptcol_x.y || scriptcol_x.y > motion_processing.scriptcol_x.x)))//!(scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)))//!(scriptcol_x.y <= motion_processing.scriptcol_x.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && touch_right == false && scriptcol_y.x >= motion_processing.scriptcol_y.y && scriptcol_y.y <= motion_processing.scriptcol_y.x))//下側ぶつかる
                {
                    square_ground_wall_down[count - 1] = false;
                }
            }


            //player_pos();

            if (square_ground_wall_right[count - 1] && (motion_processing.scriptcol_x.y < scriptcol_x.x || Mathf.Approximately(motion_processing.scriptcol_x.y, scriptcol_x.x)))//右側
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                square_ground_wall_right[count - 1] = true;
            }
            if (square_ground_wall_left[count - 1] && (motion_processing.scriptcol_x.x > scriptcol_x.y || Mathf.Approximately(motion_processing.scriptcol_x.x, scriptcol_x.y)))//左側
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                square_ground_wall_left[count - 1] = true;
                Debug.Log("1");
            }
            if (square_ground_wall_up[count - 1] && (motion_processing.scriptcol_y.y < scriptcol_y.x || Mathf.Approximately(motion_processing.scriptcol_y.y, scriptcol_y.x)))//上側
            {
                //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.y));
                square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                square_ground_wall_up[count - 1] = true;
            }
            if (square_ground_wall_down[count - 1] && (motion_processing.scriptcol_y.x > scriptcol_y.y || Mathf.Approximately(motion_processing.scriptcol_y.x, scriptcol_y.y)))//下側
            {
                //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.x, false));
                square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                square_ground_wall_down[count - 1] = true;
            }

            //動いてないけどY座標が変わって当たったことを検知
            if (dummy_transform_position.x == befor_transform_position.x && (!Mathf.Approximately(scriptcol_y.x, befor_scriptcol_y.x) || !Mathf.Approximately(scriptcol_y.y, befor_scriptcol_y.y)
                || !Mathf.Approximately(motion_processing.scriptcol_y.x, motion_processing.befor_scriptcol_y.x) || !Mathf.Approximately(motion_processing.scriptcol_y.y, motion_processing.befor_scriptcol_y.y)) &&
                scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
            {
                //右
                if (Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y))
                {
                    square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                    square_ground_wall_right[count - 1] = true;
                }

                //左
                if (Mathf.Approximately(scriptcol_x.y, motion_processing.scriptcol_x.x))
                {
                    square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                    square_ground_wall_left[count - 1] = true;
                }
            }

            //if (square_ground_wall_down[0]) Debug.Log("0");
            if (is_x_0 && is_y_0)
            {
                if (Mathf.Approximately(motion_processing.scriptcol_y.x, scriptcol_y.y) && scriptcol_x.x > motion_processing.scriptcol_x.y && scriptcol_x.y < motion_processing.scriptcol_x.x)
                {
                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                    square_ground_wall_down[count - 1] = true;
                }
            }
            else if (is_x_0 == true && is_y_0 == false)//X座標のみ
            {
                //右でも左でも

                if (dummy_transform_position.x - befor_transform_position.x > 0)//右
                {
                    //ebug.Log((Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y) || scriptcol_x.x > motion_processing.scriptcol_x.y) +""+ (befor_scriptcol_x.x <= motion_processing.scriptcol_x.y) +""+ (scriptcol_y.x > motion_processing.scriptcol_y.y) +""+ (scriptcol_y.y < motion_processing.scriptcol_y.x));

                    if ((Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y) || scriptcol_x.x > motion_processing.scriptcol_x.y) && befor_scriptcol_x.x <= motion_processing.scriptcol_x.y && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
                    {
                        square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                        //square_ground_wall_right_distance_object[count - 1] = objects[count - 1];
                        square_ground_wall_right[count - 1] = true;
                        //Debug.Log("Now");
                    }
                    else square_ground_wall_right[count - 1] = false;
                }

                if (dummy_transform_position.x - befor_transform_position.x < 0)//左
                {
                    if ((Mathf.Approximately(scriptcol_x.y, motion_processing.scriptcol_x.x) || scriptcol_x.y < motion_processing.scriptcol_x.x) && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_y.x > motion_processing.scriptcol_y.y && scriptcol_y.y < motion_processing.scriptcol_y.x)
                    {
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        //square_ground_wall_right_distance_object[count - 1] = objects[count - 1];
                        square_ground_wall_left[count - 1] = true;
                    }
                    else square_ground_wall_left[count - 1] = false;
                }


                if (Mathf.Approximately(scriptcol_y.y, motion_processing.scriptcol_y.x) && scriptcol_x.y < motion_processing.scriptcol_x.x && scriptcol_x.x > motion_processing.scriptcol_x.y)
                {
                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                    square_ground_wall_down[count - 1] = true;
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
            else if (is_y_0 == false)//XとY
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

                                    if (ObjectSettings.stop_when_this_collide)
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

                                if (ObjectSettings.stop_when_this_collide)
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
                    else if (motion_processing.scriptcol_y.x < befor_scriptcol_y.y && motion_processing.scriptcol_y.x >= scriptcol_y.y && (motion_processing.scriptcol_y.x - b3) / a3 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.x - b3) / a3 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y > 0)//先にぶつかったのは横か縦か-相手の下面-(a3/右側下)
                    {
                        if (befor_transform_position.x - dummy_transform_position.x != 0)
                        {
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//進む方向が正
                            {
                                if ((motion_processing.scriptcol_y.x - b3) / a3 > befor_scriptcol_x.x && (motion_processing.scriptcol_y.x - b3) / a3 <= scriptcol_x.x)
                                {
                                    if (ObjectSettings.stop_when_this_collide)
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

                                if (ObjectSettings.stop_when_this_collide)
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
                                if ((motion_processing.scriptcol_y.y - b2) / a2 > befor_scriptcol_x.y && (motion_processing.scriptcol_y.y - b2) / a2 <= scriptcol_x.y)
                                {
                                    //dummy_transform_position += new Vector3(((motion_processing.scriptcol_y.y - b1) / a1) - dummy_transform_position.x, (a1 * ((motion_processing.scriptcol_y.y - b1) / a1) + b1) - dummy_transform_position.y, 0);
                                    //Debug.Log("aaaa" + (motion_processing.scriptcol_y.y - b2) / a2);
                                    if (ObjectSettings.stop_when_this_collide)
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
                            else if ((motion_processing.scriptcol_y.y - b2) / a2 < befor_scriptcol_x.y && (motion_processing.scriptcol_y.y - b2) / a2 >= scriptcol_x.y)//進む方向が負
                            {
                                if (ObjectSettings.stop_when_this_collide)
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
                            if (befor_transform_position.x - dummy_transform_position.x < 0)//進む方向が正
                            {
                                if ((motion_processing.scriptcol_y.x - b4) / a4 > befor_scriptcol_x.y && (motion_processing.scriptcol_y.x - b4) / a4 <= scriptcol_x.y)
                                {
                                    if (ObjectSettings.stop_when_this_collide)
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
                                if (ObjectSettings.stop_when_this_collide)
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
                    ////a1：右上の点の比例定数
                    ////a2：左上の点の比例定数
                    ////a3：右下の点の比例定数
                    ////a4：左下の点の比例定数

                    //- 1次関数の等式変形 -

                    //y = ax + b
                    //y / x = a + b
                    //1 / x = (a + b) / y

                    //y = ax + b
                    //y - b = ax
                    //(y - b) / a = x
                    //x = (y - b) / a

                    //下方向の「横に動いてるとき、自分より細いやつが自分の底面(両頂点より内側)にぶつかったとき」
                    //if (((motion_processing.scriptcol_y.x - b4) / a4) >= motion_processing.scriptcol_x.y && ((motion_processing.scriptcol_y.x - b3) / a3) <= motion_processing.scriptcol_x.x
                    //    && motion_processing.scriptcol_y.x >= scriptcol_y.y && motion_processing.scriptcol_y.x <= befor_scriptcol_y.y
                    //    && !Mathf.Approximately(befor_scriptcol_x.x, scriptcol_x.x))
                    //{
                    //    if (stop_when_this_collide)
                    //    {
                    //        square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                    //    }
                    //    else
                    //    {
                    //        square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                    //    }
                    //    square_ground_wall_down[count - 1] = true;
                    //}
                    //上
                    //else if ((motion_processing.scriptcol_y.x - b1) / a1 < scriptcol_x.x && (motion_processing.scriptcol_y.x - ))
                }

                set_cols();

                if (motion_TransSize.y >= mysize.y || Mathf.Approximately(motion_TransSize.y, mysize.y))//相手のほうが大きいまたは同じサイズ//??
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
                    }

                    set_cols();

                    if ((a4 * motion_processing.scriptcol_x.x + b4) >= motion_processing.scriptcol_y.y && (a4 * motion_processing.scriptcol_x.x + b4) <= motion_processing.scriptcol_y.x && befor_scriptcol_x.y >= motion_processing.scriptcol_x.x && scriptcol_x.y <= motion_processing.scriptcol_x.x)//左側下ぶつかる
                    {
                        //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.x + -scriptcol_x.y, 0, 0);
                        //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        square_ground_wall_left[count - 1] = true;
                    }
                }
                else//自分のほうが大きい
                {
                    //左
                    if ((a2 * motion_processing.scriptcol_x.x + b2) > motion_processing.scriptcol_y.y && (a4 * motion_processing.scriptcol_x.x + b4) < motion_processing.scriptcol_y.x && motion_processing.scriptcol_x.x < befor_scriptcol_x.y && motion_processing.scriptcol_x.x > scriptcol_x.y)
                    {
                        square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                        square_ground_wall_left[count - 1] = true;
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
                if ((motion_processing.scriptcol_y.y > befor_scriptcol_y.x || Mathf.Approximately(motion_processing.scriptcol_y.y, befor_scriptcol_y.x)) && (Mathf.Approximately(motion_processing.scriptcol_y.y, scriptcol_y.x) || motion_processing.scriptcol_y.y <= scriptcol_y.x) && motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_x.x > scriptcol_x.y)//&& (motion_processing.scriptcol_y.y - b1) / a1 > motion_processing.scriptcol_x.y && (motion_processing.scriptcol_y.y - b1) / a1 < motion_processing.scriptcol_x.x && befor_transform_position.y - dummy_transform_position.y < 0)//先にぶつかったのは横か縦か-相手の下面-
                {
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

                if ((motion_processing.scriptcol_y.x < befor_scriptcol_y.y || Mathf.Approximately(motion_processing.scriptcol_y.x, befor_scriptcol_y.y)) && (motion_processing.scriptcol_y.x >= scriptcol_y.y || Mathf.Approximately(motion_processing.scriptcol_y.x, scriptcol_y.y)) && motion_processing.scriptcol_x.y < scriptcol_x.x && motion_processing.scriptcol_x.x > scriptcol_x.y)//先にぶつかったのは横か縦か-相手の上面-
                {
                    //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(motion_processing.scriptcol_y.x, false));
                    square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                    square_ground_wall_down[count - 1] = true;
                }
            }

            set_cols();

            if (square_ground_wall_right[count - 1] == false && Mathf.Approximately(scriptcol_x.x, motion_processing.scriptcol_x.y) && motion_processing.scriptcol_y.x > scriptcol_y.y && motion_processing.scriptcol_y.y < scriptcol_y.x)//右ぶつかる
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.y), false, 0);
                square_ground_wall_right_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.y - befor_scriptcol_x.x);
                square_ground_wall_right[count - 1] = true;
            }
            if (square_ground_wall_left[count - 1] == false && Mathf.Approximately(scriptcol_x.y, motion_processing.scriptcol_x.x) && motion_processing.scriptcol_y.x > scriptcol_y.y && motion_processing.scriptcol_y.y < scriptcol_y.x)//左ぶつかる
            {
                //dummy_transform_position += new Vector3(motion_processing.scriptcol_x.y + -scriptcol_x.x, 0, 0);
                //set_dummy_transform_position(true, change_col_to_pos_x(motion_processing.scriptcol_x.x, false), false, 0);
                square_ground_wall_left_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_x.x - befor_scriptcol_x.y);
                square_ground_wall_left[count - 1] = true;
                //Debug.Log("5");
            }
            if (square_ground_wall_up[count - 1] == false && Mathf.Approximately(scriptcol_y.x, motion_processing.scriptcol_y.y) && motion_processing.scriptcol_x.x > scriptcol_x.y && motion_processing.scriptcol_x.y < scriptcol_x.x)
            {
                square_ground_wall_up_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.y - befor_scriptcol_y.x);
                square_ground_wall_up[count - 1] = true;
            }
            if (square_ground_wall_down[count - 1] == false && Mathf.Approximately(scriptcol_y.y, motion_processing.scriptcol_y.x) && motion_processing.scriptcol_x.x > scriptcol_x.y && motion_processing.scriptcol_x.y < scriptcol_x.x)
            {
                square_ground_wall_down_distance[count - 1] = Mathf.Abs(motion_processing.scriptcol_y.x - befor_scriptcol_y.y);
                square_ground_wall_down[count - 1] = true;
            }

            count -= 1;
        }

        insertposition();

        {
            touchingCount = 0;
            //SetXCount = 0;
            //SetYCount = 0;

            if (objects.Count != 0 && ArrayUtility.Contains<bool>(square_ground_wall_right, true))
            {//右側

                float distance = Mathf.Min(square_ground_wall_right_distance);
                int count_;

                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_right_distance, distance);

                    if (!ObjectSettings.only_whether_touch)
                    {
                        //set_dummy_transform_position(true, change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.y), false, 0);
                        //if (stop_when_this_collide)
                        //{
                        //    candidate[0] = new Vector3(change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.y), change_col_to_pos_y((a1 * objects[count_].GetComponent<motion>().scriptcol_x.y) + b1), 0);
                        //}
                        //else
                        //{
                        //    candidate[0] = new Vector3(change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.y), 0, 1);
                        //}
                        touch_right = true;
                        touchingCount += 1;
                        //SetXCount += 1;
                        //candidate[0] = new PositionRelAbs
                        set_dummy_transform_position(true, change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.y), false, 0);
                        //Debug.Log("aaa");
                    }

                    if (0 < movementvalue.x)
                    {
                        movementvalue.x = 0;
                    }
                }
            }

            if (objects.Count != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_right, true))
            {
                touch_right = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (objects.Count != 0 && ArrayUtility.Contains<bool>(square_ground_wall_left, true))
            {//左側

                float distance = Mathf.Min(square_ground_wall_left_distance);
                int count_;
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_left_distance, distance);

                    if (!ObjectSettings.only_whether_touch)
                    {
                        //set_dummy_transform_position(true, change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.x, false), false, 0);
                        //if (stop_when_this_collide)
                        //{
                        //    candidate[1] = new Vector3(change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.x, false), change_col_to_pos_y((a2 * objects[count_].GetComponent<motion>().scriptcol_x.x) + b2), 0);
                        //}
                        //else
                        //{
                        //    candidate[1] = new Vector3(change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.x, false), 0, 1);
                        //}
                        touch_left = true;
                        touchingCount += 1;
                        //SetXCount += 1;
                        //candidate[1] = new PositionRelAbs
                        set_dummy_transform_position(true, change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.x, false), false, 0);
                        //Debug.Log("bbb" + change_col_to_pos_x(objects[count_].GetComponent<motion>().scriptcol_x.x, false));
                    }

                    if (movementvalue.x < 0)
                    {
                        movementvalue.x = 0;
                    }
                }
            }

            if (objects.Count != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_left, true))
            {
                touch_left = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (objects.Count != 0 && ArrayUtility.Contains<bool>(square_ground_wall_up, true))
            {//上側

                float distance = Mathf.Min(square_ground_wall_up_distance);
                int count_;

                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_up_distance, distance);

                    if (!ObjectSettings.only_whether_touch)
                    {
                        //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.y));
                        //if (stop_when_this_collide)
                        //{
                        //    candidate[2] = new Vector3(change_col_to_pos_x((objects[count_].GetComponent<motion>().scriptcol_y.y - b1) / a1), change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.y), 0);
                        //}
                        //else
                        //{
                        //    candidate[2] = new Vector3(0, change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.y), 2);
                        //}
                        touch_up = true;
                        touchingCount += 1;
                        //SetYCount += 1;
                        //candidate[2] = new PositionRelAbs
                        set_dummy_transform_position(false, 0, true, change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.y));
                    }

                    if (0 < movementvalue.y)
                    {
                        movementvalue.y = 0;
                    }
                }
            }

            if (objects.Count != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_up, true))
            {
                touch_up = false;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            if (objects.Count != 0 && ArrayUtility.Contains<bool>(square_ground_wall_down, true))
            {//下側

                float distance = Mathf.Min(square_ground_wall_down_distance);
                int count_;
                if (distance != Mathf.Infinity)
                {
                    count_ = ArrayUtility.IndexOf(square_ground_wall_down_distance, distance);
                    //Debug.Log(SetStringFromList(square_ground_wall_down_distance) + "||" + objects[count_].name + "||" + distance);

                    if (!ObjectSettings.only_whether_touch)
                    {
                        //set_dummy_transform_position(false, 0, true, change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.x, false));
                        //if (stop_when_this_collide)
                        //{
                        //    candidate[3] = new Vector3(change_col_to_pos_x((objects[count_].GetComponent<motion>().scriptcol_y.x - b3) / a3), change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.x, false), 0);
                        //}
                        //else
                        //{
                        //    candidate[3] = new Vector3(0, change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.x, false), 2);
                        //}
                        touch_down = true;
                        touchingCount += 1;
                        //SetYCount += 1;
                        //candidate[3] = new PositionRelAbs
                        set_dummy_transform_position(false, 0, true, change_col_to_pos_y(objects[count_].GetComponent<motion>().scriptcol_y.x, false));
                    }

                    if (0 > movementvalue.y)
                    {
                        movementvalue.y = 0;
                    }
                }
            }

            if (objects.Count != 0 && !ArrayUtility.Contains<bool>(square_ground_wall_down, true))
            {
                touch_down = false;
            }

            if (touch_down || touch_left || touch_right || touch_up) touching_something = true;
            else touching_something = false;

            //if (!(touchingCount == 0))
            //{
            //    Debug.Log("not0 But it's" + touchingCount);
            //}
            //if (touchingCount == 1)
            //{
            //    if (touch_up)
            //    {
            //        absolutelyX[2] = candidate[2].absX;

            //        absolutelyY[2] = candidate[2].absY;

            //        set_dummy_transform_position(absolutelyX[2], candidate[2].pos.x, absolutelyY[2], candidate[2].pos.y);
            //    }

            //    if (touch_down)
            //    {
            //        absolutelyX[3] = candidate[3].absX;

            //        absolutelyY[3] = candidate[3].absY;

            //        set_dummy_transform_position(absolutelyX[3], candidate[3].pos.x, absolutelyY[3], candidate[3].pos.y);
            //    }

            //    if (touch_left)
            //    {
            //        absolutelyX[1] = candidate[1].absX;

            //        absolutelyY[1] = candidate[1].absY;

            //        set_dummy_transform_position(absolutelyX[1], candidate[1].pos.x, absolutelyY[1], candidate[1].pos.y);
            //    }

            //    if (touch_right)
            //    {
            //        absolutelyX[0] = candidate[0].absX;

            //        absolutelyY[0] = candidate[0].absY;

            //        set_dummy_transform_position(absolutelyX[0], candidate[0].pos.x, absolutelyY[0], candidate[0].pos.y);
            //    }
            //}
            //else if (touchingCount > 1 && (SetXCount > 1 || SetYCount > 1))
            //{
            //    //上:2
            //    //下:3
            //    //左:1
            //    //右:0
            //    if (touch_up)
            //    {
            //        absolutelyX[2] = candidate[2].absX;

            //        absolutelyY[2] = candidate[2].absY;
            //        //////////////////////////////////

            //        if (absolutelyX[2]) comparison[2].x = candidate[2].pos.x;
            //        else comparison[2].x = candidate[2].pos.x + dummy_transform_position.x;

            //        if (absolutelyY[2]) comparison[2].y = candidate[2].pos.y;
            //        else comparison[2].y = candidate[2].pos.y + dummy_transform_position.y;
            //        /////////////////////////////////////

            //        if (comparison[2].x > dummy_transform_position.x) IsPositive.x = 1;
            //        else IsPositive.x = 0;

            //        if (comparison[2].y > dummy_transform_position.y) IsPositive.y = 1;
            //        else IsPositive.y = 0;
            //        //////////////////////////////////

            //        comparison[2].x -= dummy_transform_position.x;
            //        comparison[2].y -= dummy_transform_position.y;
            //        /////////////////////////////////
            //    }
            //    if (touch_down)
            //    {
            //        absolutelyX[3] = candidate[3].absX;

            //        absolutelyY[3] = candidate[3].absY;
            //        ////////////////////////////////////

            //        if (absolutelyX[3]) comparison[3].x = candidate[3].pos.x;
            //        else comparison[3].x = candidate[3].pos.x + dummy_transform_position.x;

            //        if (absolutelyY[3]) comparison[3].y = candidate[3].pos.y;
            //        else comparison[3].y = candidate[3].pos.y + dummy_transform_position.y;
            //        //////////////////////////////////////

            //        if (comparison[3].x > dummy_transform_position.x) IsPositive.x = 1;
            //        else IsPositive.x = 0;

            //        if (comparison[3].y > dummy_transform_position.y) IsPositive.y = 1;
            //        else IsPositive.y = 0;
            //        ///////////////////////////////////////

            //        comparison[3].x -= dummy_transform_position.x;
            //        comparison[3].y -= dummy_transform_position.y;
            //        //////////////////////////////////
            //    }
            //    if (touch_left)
            //    {
            //        absolutelyX[1] = candidate[1].absX;

            //        absolutelyY[1] = candidate[1].absY;
            //        //////////////////////////////////

            //        if (absolutelyX[1]) comparison[1].x = candidate[1].pos.x;
            //        else comparison[1].x = candidate[1].pos.x + dummy_transform_position.x;

            //        if (absolutelyY[1]) comparison[1].y = candidate[1].pos.y;
            //        else comparison[1].y = candidate[1].pos.y + dummy_transform_position.y;
            //        ////////////////////////////////////

            //        if (comparison[1].x > dummy_transform_position.x) IsPositive.x = 1;
            //        else IsPositive.x = 0;

            //        if (comparison[1].y > dummy_transform_position.y) IsPositive.y = 1;
            //        else IsPositive.y = 0;
            //        //////////////////////////////////////////

            //        comparison[1].x -= dummy_transform_position.x;
            //        comparison[1].y -= dummy_transform_position.y;
            //        ////////////////////////////////////////
            //    }
            //    if (touch_right)
            //    {
            //        absolutelyX[0] = candidate[0].absX;

            //        absolutelyY[0] = candidate[0].absY;
            //        ////////////////////////////////////////

            //        if (absolutelyX[0]) comparison[0].x = candidate[0].pos.x;
            //        else comparison[0].x = candidate[0].pos.x + dummy_transform_position.x;

            //        if (absolutelyY[0]) comparison[0].y = candidate[0].pos.y;
            //        else comparison[0].y = candidate[0].pos.y + dummy_transform_position.y;
            //        /////////////////////////////////////

            //        if (comparison[0].x > dummy_transform_position.x) IsPositive.x = 1;
            //        else IsPositive.x = 0;

            //        if (comparison[0].y > dummy_transform_position.y) IsPositive.y = 1;
            //        else IsPositive.y = 0;
            //        ///////////////////////////////////

            //        comparison[0].x -= dummy_transform_position.x;
            //        comparison[0].y -= dummy_transform_position.y;
            //        /////////////////////////////////////
            //    }

            //    for (int count_ = comparison.Length; count_ > 0; --count_)
            //    {
            //        comparisonX[count_ - 1] = comparison[count_ - 1].x;
            //        comparisonY[count_ - 1] = comparison[count_ - 1].y;
            //    }

            //    if (IsPositive.x == 1)
            //    {
            //        ResultOfSetDummyTransformPosition.x = Mathf.Min(comparisonX);
            //    }
            //    else
            //    {
            //        ResultOfSetDummyTransformPosition.x = Mathf.Max(comparisonX);
            //    }

            //    if (IsPositive.y == 1)
            //    {
            //        ResultOfSetDummyTransformPosition.y = Mathf.Min(comparisonY);
            //    }
            //    else
            //    {
            //        ResultOfSetDummyTransformPosition.y = Mathf.Max(comparisonY);
            //    }

            //    if (!candidate[Array.IndexOf(comparisonX, ResultOfSetDummyTransformPosition.x)].absX)
            //    {
            //        ResultOfSetDummyTransformPosition.x = candidate[Array.IndexOf(comparisonX, ResultOfSetDummyTransformPosition.x)].pos.x;
            //    }

            //    if (!candidate[Array.IndexOf(comparisonY, ResultOfSetDummyTransformPosition.y)].absY)
            //    {
            //        ResultOfSetDummyTransformPosition.y = candidate[Array.IndexOf(comparisonY, ResultOfSetDummyTransformPosition.y)].pos.y;
            //    }

            //    //if (ResultOfSetDummyTransformPosition.x == dummy_transform_position.x)
            //    //{
            //    //    WillMoveX = false;
            //    //    ResultOfSetDummyTransformPosition.x = 0;
            //    //}
            //    //else WillMoveX = true;

            //    //if (ResultOfSetDummyTransformPosition.y == dummy_transform_position.y)
            //    //{
            //    //    WillMoveY = false;
            //    //    ResultOfSetDummyTransformPosition.y = 0;
            //    //}
            //    //else WillMoveY = true;

            //    set_dummy_transform_position(candidate[Array.IndexOf(comparisonX, ResultOfSetDummyTransformPosition.x)].absX, ResultOfSetDummyTransformPosition.x, 
            //        candidate[Array.IndexOf(comparisonY, ResultOfSetDummyTransformPosition.y)].absY, ResultOfSetDummyTransformPosition.y);
            //}

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
                        touching_down[putcount] = objects[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                for (int cc = square_ground_wall_up.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_up[cc - 1])
                    {
                        touching_up[putcount] = objects[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                for (int cc = square_ground_wall_left.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_left[cc - 1])
                    {
                        touching_left[putcount] = objects[cc - 1];
                        putcount += 1;
                    }
                }

                storecount += putcount;
                putcount = 0;

                for (int cc = square_ground_wall_right.Length; cc > 0; --cc)
                {
                    if (square_ground_wall_right[cc - 1])
                    {
                        touching_right[putcount] = objects[cc - 1];
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

    private void MoveFromSet(int ToStartFrom)
    {
        int ToSetStartFrom = ToStartFrom;
        bool? B_SetBeforeCol = null;
        while (ToSetStartFrom <= dummy_transform_position_to_set_container_List.Count - 1)
        {
            if (dummy_transform_position_to_set_container_List[ToSetStartFrom].SetBeforeCol)
            {
                if (B_SetBeforeCol == false) dummy_transform_position = befor_transform_position;
                B_SetBeforeCol = true;
                check_change_dummy_transform_position(dummy_transform_position_to_set_container_List[ToSetStartFrom].Values);
                dummy_transform_position = dummy_transform_position_to_set_container_List[ToSetStartFrom].Values;
                ToSetStartFrom++;
            }
            else
            {
                if (B_SetBeforeCol == true) Main();
                B_SetBeforeCol = false;
                dummy_transform_position = dummy_transform_position_to_set_container_List[ToSetStartFrom].Values;
                ToSetStartFrom++;
            }
        }
        do_dummy_transform_position_to_set_execute = false;
    }

    public void Addmovement(Vector2 MovementValue)
    {
        //movement(new Vector2(movementvalue.x + MovementValue.x, movementvalue.y + MovementValue.y), true);
        movementvalue += MovementValue;
    }

    public void Movement(Vector2 movementvalueforset, bool set = true)
    {
        bool numplus_x = true;


        if (set == true)
        {
            //set = false;
            movementvalue = movementvalueforset;
            if (movementvalueforset.x > 0)
            {
                numplus_x = true;
            }
            else numplus_x = false;

            //if (movementvalue.x == 0.3f) Debug.Log("go right");
        }

        motion motion__processing;
        float[] posY = new float[objects.Count];
        //float[] posYToSave;
        bool yTrue = false;

        float InsteadOfMovementY = movementvalue.y;

        bool IsMovementvaluePositive = true;

        if (movementvalue.y > 0) IsMovementvaluePositive = true;
        else if (movementvalue.y < 0) IsMovementvaluePositive = false;

        for (int c = objects.Count; c > 0; --c)
        {
            if (!IsMovementvaluePositive) posY[c - 1] = Mathf.NegativeInfinity;
            else posY[c - 1] = Mathf.Infinity;
        }

        for (int c = objects.Count; c > 0; --c)
        {
            //try
            //{
            motion__processing = objects[c - 1].GetComponent<motion>();
            //}
            //catch (UnassignedReferenceException a)
            //{
            //    continue;
            //}

            if (!IsMovementvaluePositive)
            {
                if (motion__processing.scriptcol_y.x < scriptcol_y.y && motion__processing.scriptcol_y.x >= (scriptcol_y.y + movementvalue.y) && motion__processing.scriptcol_x.x > scriptcol_x.y && motion__processing.scriptcol_x.y < scriptcol_x.x)
                {
                    posY[c - 1] = change_col_to_pos_y(motion__processing.scriptcol_y.x, false);
                    yTrue = true;
                }
            }
            else
            {
                if (motion__processing.scriptcol_y.y > scriptcol_y.x && motion__processing.scriptcol_y.y <= (scriptcol_y.x + movementvalue.y) && motion__processing.scriptcol_x.x > scriptcol_x.y && motion__processing.scriptcol_x.y < scriptcol_x.x)
                {
                    posY[c - 1] = change_col_to_pos_y(motion__processing.scriptcol_y.y);
                    yTrue = true;
                }
            }
        }

        //posYToSave = posY;

        //for (int c = objects.Count; c > 0; --c)
        //{
        //    if (IsMovementvaluePositive) posY[c - 1] -= scriptcol_y.x;
        //    else posY[c - 1] -= posY[c - 1] -= scriptcol_y.y;
        //}

        if (yTrue)
        {
            if (!IsMovementvaluePositive)
            {
                InsteadOfMovementY = Mathf.Max(posY);
                //change_col_to_pos_y(objects[ArrayUtility.IndexOf(posY, Mathf.Max(posY))].GetComponent<motion>().scriptcol_y.x, false);
            }
            else
            {
                InsteadOfMovementY = Mathf.Min(posY);
                //change_col_to_pos_y(objects[ArrayUtility.IndexOf(posY, Mathf.Min(posY))].GetComponent<motion>().scriptcol_y.y, true);
            }
        }

        if (touch_left && movementvalue.x < 0) movementvalue.x = 0;
        if (touch_right && movementvalue.x > 0) movementvalue.x = 0;
        if (touch_down && movementvalue.y < 0) movementvalue.y = 0;
        if (touch_up && movementvalue.y > 0) movementvalue.y = 0;

        //dummy_transform_position += new Vector3(movementvalue.x, movementvalue.y, 0);
        if (movementvalue.x != 0 || movementvalue.y != 0) Change_dummy_transform_position(false, movementvalue.x, yTrue, InsteadOfMovementY);

        if (ObjectSettings.Air_resistance != 0)
        {
            bool plus;

            if (ObjectSettings.Air_resistance > 0) plus = true;
            else plus = false;

            if (plus == true) movementvalue.x += -ObjectSettings.Air_resistance;
            else movementvalue.x += ObjectSettings.Air_resistance;

            if (plus == true && numplus_x == true && movementvalue.x < 0) movementvalue.x = 0;
            if (plus == true && numplus_x == false && movementvalue.x > 0) movementvalue.x = 0;
            //if (plus == false && movementvalue.x > 0) movementvalue.x = 0;
        }

        if (ObjectSettings.gravity != 0 && ((ObjectSettings.gravity > 0 && !touch_down) || (ObjectSettings.gravity < 0 && !touch_up)) && ObjectSettings.useGravity)
        {
            movementvalue.y -= ObjectSettings.gravity;
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

        before_touching_something = touching_something;
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

    //public void make_an_appointment_changing_dummy(bool absolute_x, float x, bool absolute_y, float y, bool absolute_z = false, float z = 0, bool movinig_without_col = false)
    //{

    //}

    public void Change_dummy_transform_position(bool absolute_x, float x, bool absolute_y, float y, bool absolute_z = false, float z = 0, bool movinig_without_col = false)
    {
        calledCount++;
        if (!do_dummy_transform_position_to_set_execute) dummy_transform_position_to_set = dummy_transform_position;
        else dummy_transform_position_to_set = dummy_transform_position_to_set_container_List[dummy_transform_position_to_set_container_List.Count - 1].Values;
        dummy_transform_position_to_set_container_List.Add(new dummy_transform_position_to_set_container(new Vector3(absolute_x ? x : dummy_transform_position_to_set.x + x, absolute_y ? y : dummy_transform_position_to_set.y + y, absolute_z ? z : dummy_transform_position_to_set.z + z), !movinig_without_col));
        //dummy_transform_position_to_set = dummy_transform_position;

        ////set x
        //if (absolute_x) dummy_transform_position_to_set.x = x;
        //else dummy_transform_position_to_set.x += x;

        ////set y
        //if (absolute_y) dummy_transform_position_to_set.y = y;
        //else dummy_transform_position_to_set.y += y;

        ////set z
        //if (absolute_z) dummy_transform_position_to_set.z = z;
        //else dummy_transform_position_to_set.z += z;

        set_befor_col += !movinig_without_col ? 1 : 0;
        do_dummy_transform_position_to_set_execute = true;

        //dummy_transform_position_to_set_container_List.Add(new dummy_transform_position_to_set_container(new bool3(absolute_x, absolute_y, absolute_z), new Vector3(x, y, z), !movinig_without_col));
    }

    public bool Is_touching(GameObject something)
    {
        if (ArrayUtility.Contains(touching, something))
        {
            return true;
        }
        else return false;
    }

    public bool Searching_object_withTag(string tag)
    {
        int c = 0;
        for (int count__ = touching.Length; count__ > 0; --count__)
        {
            if (touching[count__ - 1].tag == tag) c += 1;
        }
        //if (c > 0) return true;
        //else return false;
        return c > 0;// ? true : false;
    }

    public GameObject Touching_Object(int num)
    {
        return touching[num];
    }

    public void set_dummy_transform_position(bool absolute_x, float x, bool absolute_y, float y, bool absolute_z = false, float z = 0)//, bool movinig_without_col = false)
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

    Vector3 check_change_dummy_transform_position(Vector3 pos)
    {
        if (touch_down && dummy_transform_position.y > pos.y)
        {
            pos.y = dummy_transform_position.y;
        }

        if (touch_up && dummy_transform_position.y < pos.y)
        {
            pos.y = dummy_transform_position.y;
        }

        if (touch_left && dummy_transform_position.x > pos.x)
        {
            pos.x = dummy_transform_position.x;
        }

        if (touch_right && dummy_transform_position.x < pos.x)
        {
            pos.x = dummy_transform_position.x;
        }

        return pos;
    }

    //    (dummy_transform_position.x + (box2d.size.x* (transform.localScale.x* 0.5f)))

    //dummy.x + box2d.s.x(0.5*trans.s.x)=a
    //dummy.x + box2d.s.x((trans.s.x)/2)=a
    //dummy.x + ((box2d.s.x * trans.s.x)/2)=a
    //dummy.x = a - ((box2d.s.x * trans.s.x)/2)

    public float change_col_to_pos_x(float colx, bool is_x = true)
    {
        if (is_x) return colx - LocalScriptCol_X_IncludeRotate.x;//((box2d.size.x * transform.localScale.x) / 2);
        else return colx - LocalScriptCol_X_IncludeRotate.y;//((box2d.size.x * transform.localScale.x) / 2);
    }

    public float change_col_to_pos_y(float coly, bool is_x = true)
    {
        if (is_x) return coly - LocalScriptCol_Y_IncludeRotate.x;//LocalScriptCol_Y.x;//((box2d.size.y * transform.localScale.y) / 2);
        else return coly - LocalScriptCol_Y_IncludeRotate.y;//LocalScriptCol_Y.y;//((-box2d.size.y * transform.localScale.y) / 2);///////////////////////////////////
    }

    Vector2? WhereDoLinesCross(CollisionLine L1, CollisionLine L2, Vector2? StartingPoint = null, Vector2? EndingPoint = null)
    {
        if (L2.NotMove) return null;
        //Debug.Log(L1.a+"、"+ L1.b+ "、" + L2 + "、" + new Vector2((L1.b - L2.b) / (L2.a - L1.a), L1.a * ((L1.b - L2.b) / (L2.a - L1.a)) + L1.b));
        Vector2 tempX;
        Vector2 tempY;

        Vector2 calculated = new Vector2();
        if (L1.NotMove)
        {
            return null;
        }
        else if ((L1.xEqual0 && L2.xEqual0) || (L1.yEqual0 && L2.yEqual0))
        {
            return null;
        }
        else if (!L1.xEqual0 && !L1.yEqual0 && !L2.xEqual0 && !L2.yEqual0)
        {
            calculated = new Vector2((L1.b - L2.b) / (L2.a - L1.a), L1.a * ((L1.b - L2.b) / (L2.a - L1.a)) + L1.b);
        }
        else if (L1.xEqual0 && !L2.xEqual0)
        {
            if (L2.yEqual0)
            {
                return new Vector2(L1.b, L2.b);
            }
            else
            {
                calculated = new Vector2((L1.b - L2.b) / L2.a, L1.b);
            }
        }
        else if (L1.yEqual0 && !L2.yEqual0)
        {
            if (L2.xEqual0)
            {
                return new Vector2(L1.b, L2.b);
            }
            else
            {
                calculated = new Vector2(L1.b, L2.a * L1.b + L2.b);
            }
        }
        else Debug.LogError("想定されていない、L1:" + L1.a + "、" + L1.b + "、" + L1.xEqual0 + "、" + L1.yEqual0 + "、" + L1.NotMove + "、L2:" + L2.a + "、" + L2.b + "、" + L2.xEqual0 + "、" + L2.yEqual0 + "、" + L2.NotMove);

        if (StartingPoint != EndingPoint && (StartingPoint == null || EndingPoint == null))
        {
            Debug.LogError("You must assign both StartingPoint and EndingPoint when you want to.");
            return null;
        }
        else if (StartingPoint != null && EndingPoint != null)
        {
            tempX = new Vector2(StartingPoint.Value.x, EndingPoint.Value.x);
            tempY = new Vector2(StartingPoint.Value.y, EndingPoint.Value.y);

            try
            {
                if (Mathf.Approximately(L1.a, L2.a)) return null;
                else if (tempX.x <= calculated.x && calculated.x <= tempX.y
                    && tempY.x <= calculated.y && calculated.y <= tempY.y)
                {
                    return calculated;
                }
            }
            catch (NullReferenceException)
            {
                return null;
            }
        }
        return null;
        //return Mathf.Approximately(L1.a, L2.a) ? null : new Vector2((L1.b - L2.b) / (L2.a - L1.a), L1.a * ((L1.b - L2.b) / (L2.a - L1.a)) + L1.b);
    }

    void colcount()
    {
        square_ground_wall_down = new bool[objects.Count];

        square_ground_wall_up = new bool[objects.Count];

        square_ground_wall_right = new bool[objects.Count];

        square_ground_wall_left = new bool[objects.Count];

        square_ground_wall_down_distance = new float[objects.Count];

        square_ground_wall_up_distance = new float[objects.Count];

        square_ground_wall_right_distance = new float[objects.Count];

        square_ground_wall_left_distance = new float[objects.Count];

        touching = new GameObject[objects.Count];

        touching_down = new GameObject[objects.Count];

        touching_up = new GameObject[objects.Count];

        touching_right = new GameObject[objects.Count];

        touching_left = new GameObject[objects.Count];
    }

    void drowline(Vector3 start, Vector3 end, Color color, float time = 2, bool hide = true)
    {
        Debug.DrawLine(start, end, color, time, !hide);
    }

    static bool elementIsNull(GameObject element)
    {
        return !element;
    }

    bool RangeCheck<T>(List<T> arr, int index, [CallerLineNumber] int CalledFrom = 0)//, [CallerMemberName] string CalledBy = "")
    {
        if (index < arr.Count && !(index < 0))
        {
            //Debug.Log(CalledFrom + "行目クリア");
            return true;
        }
        else
        {
            //Debug.Log("indexが範囲外です(" + CalledFrom + "行目)");
            return false;
        }
    }

    private string SetStringFromList(float[] list)
    {
        string s = "";
        string s_;
        for (int c = list.Length; c > 0; --c)
        {
            try
            {
                s_ = String.Concat(String.Concat(objects[c - 1].name.ToString(), " = "), list[c - 1].ToString()) + ", ";
            }
#pragma warning disable CS0168 // 変数は宣言されていますが、使用されていません
            catch (UnassignedReferenceException a)
#pragma warning restore CS0168 // 変数は宣言されていますが、使用されていません
            {
                continue;
            }
            s = s + s_;
        }
        return s;
    }
}
