using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour {

    public Camera MainCamera;
    public GameObject Cube0;
    public GameObject Cube1;
    public GameObject Cube2;
    public GameObject Cube3;

    public float moveTime = 0.5f;

    public float defautDistance = 5.0f;

    private bool val0 = true;
    private bool val1 = true;
    private bool val2 = false;
    private bool val3 = false;

    private Vector3 start = Vector3.zero;
    private Vector3 end = Vector3.zero;
    private float movingTime = 0.0f;

	void Update () {
        if (start == Vector3.zero) return;

        movingTime += Time.deltaTime;
        MainCamera.transform.position = Vector3.Slerp(start, end, (movingTime / moveTime));

        if (moveTime < movingTime)
        {
            MainCamera.transform.position = end;
            start = Vector3.zero;
            end = Vector3.zero;
        }
    }

    private void OnGUI()
    {
        val0 = GUI.Toggle(new Rect(Screen.width - 100, 10, 90, 20), val0, "Cube0");
        val1 = GUI.Toggle(new Rect(Screen.width - 100, 30, 90, 20), val1, "Cube1");
        val2 = GUI.Toggle(new Rect(Screen.width - 100, 50, 90, 20), val2, "Cube2");
        val3 = GUI.Toggle(new Rect(Screen.width - 100, 70, 90, 20), val3, "Cube3");
        var btn = GUI.Button(new Rect(Screen.width - 100, 90, 90, 20), "Focus");
        if (btn) OnFocus();
    }

    private void OnFocus()
    {
        var targets = new List<GameObject>();
        if (val0) targets.Add(Cube0);
        if (val1) targets.Add(Cube1);
        if (val2) targets.Add(Cube2);
        if (val3) targets.Add(Cube3);
        var targetPositons = targets.Select(x => x.transform.position).ToList();

        // すべての物体の中心位置を取得
        var targetCenter = new Vector3(
            (targetPositons.Max(x => x.x) + targetPositons.Min(x => x.x)) / 2,
            (targetPositons.Max(x => x.y) + targetPositons.Min(x => x.y)) / 2,
            (targetPositons.Max(x => x.z) + targetPositons.Min(x => x.z)) / 2
        );
        // Debug.Log(string.Format("targetCenter={0}", targetCenter));

        // 物体中心からの距離を計算
        var v_distance = defautDistance;
        // Debug.Log(string.Format("v_distance={0}", v_distance));
        if (2 <= targetPositons.Count)
        {
            // 仮想座標：基準線(カメラ->座標の中心)
            var kizyunsen = new Vector3(0, 0, v_distance);

            // 仮想座標：評価線(カメラ->各オブジェクトの中心)
            var hyoukasen = new List<Vector3>();
            foreach(var tp in targetPositons)
            {
                var line = kizyunsen + (tp - targetCenter);
                // Debug.Log(string.Format("kizyun={0} line={1} angle={2}", kizyunsen, line, Vector3.Angle(kizyunsen, line)));
                hyoukasen.Add(line);
            }

            // 基準線と評価線の角度を取得
            var angles = hyoukasen.Select(x => Vector3.Angle(kizyunsen, x)).ToList();

            // 仮想座標上で距離を決定
            v_distance = v_distance / MainCamera.fieldOfView * (angles.Max() * 2.5f);
            // 仮想座標上で距離を決定（補正してみる）
            // var cen_distance = (targetCenter - MainCamera.transform.position).magnitude;
            // var min_distance = targetPositons.Select(x => (x - MainCamera.transform.position).magnitude).Min();
            // v_distance = v_distance + (cen_distance - min_distance);
            // Debug.Log(string.Format("v_distance={0}", v_distance));
        }

        // カメラの移動先を設定
        var relative = MainCamera.transform.TransformPoint(new Vector3(0, 0, v_distance));
        // Debug.Log(string.Format("{0} {1}", MainCamera.transform.position, relative));
        this.start = MainCamera.transform.position;
        this.end = targetCenter - (relative - MainCamera.transform.position);
        this.movingTime = 0.0f;
    }

    /// <summary>
    /// グローバル座標系で動いたときのローカル座標の移動量を取得する
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public Vector3 GetCameraRelativePostion(Vector3 targetPos)
    {
        var dire = targetPos - MainCamera.transform.position;
        return MainCamera.transform.InverseTransformDirection(dire);
    }
}
