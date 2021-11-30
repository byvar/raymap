using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenSpace;
using UnityEngine;

public class LineRouteCreator : MonoBehaviour
{
    public LineRenderer LineRenderer;

    public List<Vector3> Points = new List<Vector3>();

    void OnEnable()
    {
        foreach (var p in MapLoader.Loader.persos.Where(p=>p.namePerso.StartsWith("ZOR_LumJaune"))) {
            Points.Add(p.Gao.transform.position);
        }

        Points = SolveTSP(Points);


        LineRenderer.positionCount = Points.Count;
        LineRenderer.SetPositions(Points.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            Points.Add(Camera.main.transform.position);

            LineRenderer.positionCount = Points.Count;
            LineRenderer.SetPositions(Points.ToArray());
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            Points.Clear();

            LineRenderer.positionCount = 0;
            LineRenderer.SetPositions(Points.ToArray());
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            var newPoints = SolveTSP(Points);//Points.GetRange(1,Points.Count-2));
            //newPoints.Insert(0,Points[0]);
            //newPoints.Add(Points.Last());

            Points = newPoints;
            LineRenderer.SetPositions(Points.ToArray());
        }

    }

    public struct Option
    {
        public int[] Order;
        public float Distance;

        public Option(int[] order, float distance)
        {
            Order = order;
            Distance = distance;
        }
    }

    // Brute force traveling salesman problem
    private List<Vector3> SolveTSP(List<Vector3> points)
    {
        float CalculateDistance(int[] a)
        {
            float dist = 0;
            for (int i = 0; i < a.Length-1; i++) {
                dist += Vector3.Distance(points[a[i]], points[a[i+1]]);
            }

            return dist;
        }

        void HeapPermutation(int[] a, int size, ref List<Option> options)
        {
            // if size becomes 1 then prints the obtained
            // permutation
            if (size == 1) {

                options.Add(new Option()
                {
                    Order = (int[])a.Clone(),
                    Distance = CalculateDistance(a),
                });
            }

            for (int i = 0; i < size; i++) {
                HeapPermutation(a, size - 1, ref options);

                // if size is odd, swap 0th i.e (first) and
                // (size-1)th i.e (last) element
                if (size % 2 == 1) {
                    int temp = a[0];
                    a[0] = a[size - 1];
                    a[size - 1] = temp;
                }

                // If size is even, swap ith and
                // (size-1)th i.e (last) element
                else {
                    int temp = a[i];
                    a[i] = a[size - 1];
                    a[size - 1] = temp;
                }
            }

        }

        List<Option> options = new List<Option>();
        int[] order = new int[points.Count];
        for (int i = 0; i < order.Length; i++) {
            order[i] = i;
        }
        HeapPermutation(order, order.Length, ref options);
        var sortedOptions = options.OrderBy(o=>o.Distance);

        int c = 0;
        foreach (var option in sortedOptions) {
            Debug.Log($"Option #{c++}, order is {string.Join(",", option.Order)} distance is "+option.Distance);
        }

        var bestOption = sortedOptions.First();

        List<Vector3> newPoints = new List<Vector3>();
        for (int i = 0; i < points.Count; i++) {
            newPoints.Add(points[bestOption.Order[i]]);
        }

        return newPoints;
    }

   
}
