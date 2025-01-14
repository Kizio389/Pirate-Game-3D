using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : MonoBehaviour
{
    public string itemName; // Tên item cần chế tạo

    public string Req1; // Nguyên liệu 1
    public string Req2; // Nguyên liệu 2

    public int Req1amount; // Số lượng yêu cầu của nguyên liệu 1
    public int Req2amount; // Số lượng yêu cầu của nguyên liệu 2

    public int numOfRequirements; // Tổng số loại nguyên liệu cần thiết

    // Constructor để khởi tạo Blueprint với các thông tin cần thiết
    public Blueprint(string name, int reqNUM, string R1, int R1num, string R2, int R2num)
    {
        itemName = name;
        numOfRequirements = reqNUM; // Số lượng loại nguyên liệu (ví dụ: 2 loại)
        Req1 = R1;                 // Nguyên liệu 1
        Req2 = R2;                 // Nguyên liệu 2
        Req1amount = R1num;        // Số lượng yêu cầu của nguyên liệu 1
        Req2amount = R2num;        // Số lượng yêu cầu của nguyên liệu 2
    }
}
