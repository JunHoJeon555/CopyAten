using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_AStar : Test_Base
{
    private void Start()
    {
        GridMap map = new GridMap(4, 3);

        Node node = map.GetNode(1, 0);
        node.gridType = Node.GridType.Wall;
        node = map.GetNode(2, 2);
        node.gridType = Node.GridType.Wall;

        List<Vector2Int> path = AStar.PathFind(map, new(0, 0), new(3, 2));

        string pathStr = "Path : ";
        foreach (var pos in path)
        {
            pathStr += $" ({pos.x}, {pos.y}) ->";
        }
        pathStr += " 끝";
        Debug.Log(pathStr);

    }

    void Test_Sort()
    {
        // Operator (연산자, 명령어)
        // +-*/     : 산술 연산자
        // && ||    : 논리 연산자
        // & |      : 비트 연산자
        // > < >= <= : 비교 연산자
        // =        : 대입 연산자

        //List<int> list = new List<int>();
        //list.Add(1);
        //list.Add(10);
        //list.Add(5);
        //list.Add(7);
        //list.Add(8);

        //foreach(int num in list)
        //{
        //    Debug.Log(num);
        //}

        //list.Sort();

        //foreach (int num in list)
        //{
        //    Debug.Log(num);
        //}

        //Node a = new Node(0, 0);
        //a.G = 10;
        //Node b = new Node(1, 1, Node.GridType.Wall);
        //b.G = 30;
        //Node c = new Node(2, 2);
        //c.G = 15;

        //List<Node> list = new List<Node>();
        //list.Add(a);
        //list.Add(b);
        //list.Add(c);

        //foreach (Node node in list)
        //{
        //    Debug.Log(node.F);
        //}

        //list.Sort();

        //foreach (Node node in list)
        //{
        //    Debug.Log(node.F);
        //}
    }
}
