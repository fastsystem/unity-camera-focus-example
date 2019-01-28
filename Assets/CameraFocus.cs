using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour {

    public Camera MainCamera;
    public GameObject White;
    public GameObject Red;
    public GameObject Green;
    public GameObject Blue;

    public float moveTime = 0.5f;

    public float defautDistance = 5.0f;

    private bool val0 = true;
    private bool val1 = true;
    private bool val2 = false;
    private bool val3 = false;

    private Vector3? start = null;
    private Vector3? end = null;
    private float movingTime = 0.0f;

	void Update () {
        if (start == null) return;

        movingTime += Time.deltaTime;
        MainCamera.transform.position = Vector3.Slerp(start.Value, end.Value, (movingTime / moveTime));

        if (moveTime < movingTime)
        {
            MainCamera.transform.position = end.Value;
            start = null;
            end = null;
        }
    }

    private void OnGUI()
    {
        val0 = GUI.Toggle(new Rect(Screen.width - 100, 10, 90, 20), val0, "White");
        val1 = GUI.Toggle(new Rect(Screen.width - 100, 30, 90, 20), val1, "Red");
        val2 = GUI.Toggle(new Rect(Screen.width - 100, 50, 90, 20), val2, "Blue");
        val3 = GUI.Toggle(new Rect(Screen.width - 100, 70, 90, 20), val3, "Green");
        var btn = GUI.Button(new Rect(Screen.width - 100, 90, 90, 20), "Focus");
        if (btn)
        {
            var targets = new List<GameObject>();
            if (val0) targets.Add(White);
            if (val1) targets.Add(Red);
            if (val2) targets.Add(Blue);
            if (val3) targets.Add(Green);
            if (targets.Count == 0) return;

            this.Focus(MainCamera, targets);
        }
    }

    public void Focus(Camera camera, List<GameObject> targets)
    {
        // 物体が１つの場合は対象の点を１つにする
        List<Vector3> targetPositons = new List<Vector3>();
        if (1 == targets.Count)
            targetPositons.Add(targets[0].transform.position);
        else
            foreach (var t in new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back })
                targetPositons.AddRange(targets.Select(x => x.transform.position + t));

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
            foreach (var tp in targetPositons)
            {
                var line = kizyunsen + (tp - targetCenter);
                // Debug.Log(string.Format("kizyun={0} line={1} angle={2}", kizyunsen, line, Vector3.Angle(kizyunsen, line)));
                hyoukasen.Add(line);
            }

            // 基準線と評価線の角度を取得
            var angles = hyoukasen.Select(x => Vector3.Angle(kizyunsen, x)).ToList();

            // 仮想座標上で距離を決定
            v_distance = v_distance / camera.fieldOfView * (angles.Max() * 2.0f);
            // 仮想座標上で距離を決定（補正してみる）
            // var cen_distance = (targetCenter - camera.transform.position).magnitude;
            // var min_distance = targetPositons.Select(x => (x - camera.transform.position).magnitude).Min();
            // v_distance = v_distance + (cen_distance - min_distance);
            // Debug.Log(string.Format("v_distance={0}", v_distance));
        }

        // カメラの移動先を設定
        var relative = camera.transform.TransformPoint(new Vector3(0, 0, v_distance));
        // Debug.Log(string.Format("{0} {1}", camera.transform.position, relative));
        this.start = camera.transform.position;
        this.end = targetCenter - (relative - camera.transform.position);
        this.movingTime = 0.0f;
    }
}
