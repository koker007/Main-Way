using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calc
{
    public class Array {
        public static int[] unite(int[] a, int[] b) {
            int[] result = new int[a.Length + b.Length];

            for (int num = 0; num < a.Length; num++) {
                result[num] = a[num];
            }

            for (int num = 0; num < b.Length; num++)
            {
                result[a.Length + num] = b[num];
            }

            return result;
        }

        public static Vector2[] unite(Vector2[] a, Vector2[] b)
        {
            Vector2[] result = new Vector2[a.Length + b.Length];

            for (int num = 0; num < a.Length; num++)
            {
                result[num] = a[num];
            }

            for (int num = 0; num < b.Length; num++)
            {
                result[a.Length + num] = b[num];
            }

            return result;
        }
        public static Vector3[] unite(Vector3[] a, Vector3[] b)
        {
            Vector3[] result = new Vector3[a.Length + b.Length];

            for (int num = 0; num < a.Length; num++)
            {
                result[num] = a[num];
            }

            for (int num = 0; num < b.Length; num++)
            {
                result[a.Length + num] = b[num];
            }

            return result;
        }
    }


    public static int Nolmalize(float value) {
        int ret = 0;
        if (value > 0) {
            ret = 1;
        }
        else if (value < 0) {
            ret = -1;
        }
        return ret;
    }
    public static float CutFrom0To1(float value) {
        float ret = value;
        if (ret > 1) {
            ret = 1;
        }
        else if (ret < 0) {
            ret = 0;
        }

        return ret;
    }

    public static float FixRandom(float fixNum, float min, float max) {
        float scale = Mathf.Abs(max - min);
        float perlinPos = fixNum / 3;
        float ret = Mathf.PerlinNoise(perlinPos, Mathf.PI);

        ret *= 100;
        ret = ret % 1;
        ret = ret * scale;
        ret += fixNum % scale;
        if (ret > scale) {
            ret -= scale;
        }

        if (min < max)
        {
            ret = min + ret;
        }
        else {
            ret = max + ret;
        }

        return ret;
    }


    //Получить число на основе сида и второго компонента
    public static float GetSeedNum(string Seed, int part)
    {
        float result = 0;

        float numerator =  100;
        float denominator = 1;

        bool isSum = true;

        float plus = Mathf.Sin(Mathf.PI * part) * 43758.5453123f;

        //получить рандомное число на основе сида
        for (int x = 0; x < Seed.Length; x++) {

            int num = part + x;

            num += (int)(Mathf.Sin(num * 747)*43758.5453123f);

            //число должно быть положительным
            //чтобы не вышли за рамки массива
            num = Mathf.Abs(num);
            num = num % Seed.Length;

            //Делитель равняется новому значению
            denominator = Seed[num];

            //Вычисляем
            float division = numerator / denominator;

            if (isSum)
                result += (Mathf.Sin(Mathf.PI * division) * 43758.5453123f)/10000;
            else
                result -= (Mathf.Sin(Mathf.PI * division) * 43758.5453123f)/10000;

            //убираем целое
            result %= 1;

            //Меняем переключаетель вычитания или прибавления
            isSum = !isSum;

            //Вычисления выполненны
            //Делитель становится числителем
            numerator = denominator;
        }

        result += plus;
        result %= 1;

        //Когда результат есть
        return result;
    }

    public static Size GetSize(float size) {
        if (size >= 65536) return Size.s65536;
        if (size >= 32768) return Size.s32768;
        if (size >= 16384) return Size.s16384;
        if (size >= 8192) return Size.s8192;
        if (size >= 4096) return Size.s4096;
        if (size >= 2048) return Size.s2048;
        if (size >= 1024) return Size.s1024;
        if (size >= 512) return Size.s512;
        if (size >= 256) return Size.s256;
        if (size >= 128) return Size.s128;
        if (size >= 64) return Size.s64;
        if (size >= 32) return Size.s32;
        if (size >= 16) return Size.s16;
        if (size >= 8) return Size.s8;
        if (size >= 4) return Size.s4;
        if (size >= 2) return Size.s2;
        else return Size.s1;
    }

    public static int GetSizeInt(Size size)
    {
        if ((int)size >= 17) return 65536;
        if ((int)size >= 16) return 32768;
        if ((int)size >= 15) return 16384;
        if ((int)size >= 14) return 8192;
        if ((int)size >= 13) return 4096;
        if ((int)size >= 12) return 2048;
        if ((int)size >= 11) return 1024;
        if ((int)size >= 10) return 512;
        if ((int)size >= 9) return 256;
        if ((int)size >= 8) return 128;
        if ((int)size >= 7) return 64;
        if ((int)size >= 6) return 32;
        if ((int)size >= 5) return 16;
        if ((int)size >= 4) return 8;
        if ((int)size >= 3) return 4;
        if ((int)size >= 2) return 2;
        else return 1;
    }
}

//Класс линии
public class Line {
    Vector3 pointA = new Vector3();
    Vector3 pointB = new Vector3();

    public Vector3 PointA {
        get
        {
            return pointA;
        }
    }
    public Vector3 PointB {
        get {
            return pointB;
        }
    }

    public Line(Vector3 pointA, Vector3 pointB) {
        this.pointA = pointA;
        this.pointB = pointB;
    }
    public Line(Vector3 pointB) {
        this.pointA = new Vector3();
        this.pointB = pointB;
    }

    public Vector3 vector {
        get
        {
            return pointB - pointA;
        }
    }

    /// <summary>
    /// Получить кратчайшие точки между прямыми
    /// </summary>
    /// <returns></returns>
    static public Line GetNearestLine(Line line1, Line line2) {


        Vector3 vec1 = line1.vector;
        Vector3 vec2 = line2.vector;

        //Создаем плоскость паралельную 1 линии
        Plane plane2 = new Plane(line2.pointA, line2.pointB, line2.pointA + vec1);
        //Получаем нормаль плоскости 2
        Vector3 plane2Normal = plane2.normal;

        //Нужно найти точку соприкосновения от линии1А по нормали к плоскости2
        //Создаем луч направленный на плоскость
        Ray rayToPlane2Normal = new Ray(line1.pointA, plane2Normal);
        float distEnderNormalPlane2 = 0;
        bool enter = plane2.Raycast(rayToPlane2Normal, out distEnderNormalPlane2);

        //Узнаем координаты точки соприкосновения на плоскости plane2
        Vector3 pointF2 = line1.pointA + plane2Normal * distEnderNormalPlane2;

        Debug.Log("Point: " + pointF2);

        //Теперь создаем плоскость где линия 1 может быть нормалью
        Plane plane2normLine1 = new Plane(line2.pointA, line2.pointB, line2.pointA + plane2Normal);
        //Создаем луч направленный на плоскость
        Ray rayToPlane2line1 = new Ray(pointF2, pointF2 + vec1);
        float distEnderPlane2line = 0;
        bool enter2 = plane2.Raycast(rayToPlane2Normal, out distEnderPlane2line);

        //Смещаем координаты F2 на line2
        pointF2 = pointF2 + distEnderPlane2line * vec1;

        Vector3 pointF1 = line1.pointA + distEnderPlane2line * vec1;


        Debug.Log("PointF1: " + pointF1 + " PointF2: " + pointF2);

        Line resultLine = new Line(new Vector3(0,0,1));
        return resultLine;
    }

    static public Line GetNearestLine(Line line1, Line line2, int cycle)
    {

        //Берем на этих двух прямых по 2 рандомные точки
        Vector3 vec1 = line1.vector;
        Vector3 vec2 = line2.vector;

        Vector3 pointA = line1.PointA;
        Vector3 pointB = line2.PointA;

        float stepA = line1.vector.magnitude;
        float stepB = line2.vector.magnitude;

        //Уменьшаем растояние через цикл
        for (int num = cycle; num > 0; num--) {
            stepA *= 0.90f;
            stepB *= 0.90f;

            Vector3 pointAP = pointA + vec1 * stepA;
            Vector3 pointAM = pointA - vec1 * stepA;
            Vector3 pointBP = pointB + vec2 * stepB;
            Vector3 pointBM = pointB - vec2 * stepB;

            //Узнаем растояние каждого из 4-х движений
            float distPP = Vector3.Distance(pointAP, pointBP);
            float distPM = Vector3.Distance(pointAP, pointBM);
            float distMP = Vector3.Distance(pointAM, pointBP);
            float distMM = Vector3.Distance(pointAM, pointBM);

            //ищем лучшее
            float best = distPP;
            if (best > distPM) best = distPM;
            if (best > distMP) best = distMP;
            if (best > distMM) best = distMM;

            //Применяем изменения лучшего
            if (best == distPP) {
                pointA = pointAP;
                pointB = pointBP;
            }
            else if (best == distPM)
            {
                pointA = pointAP;
                pointB = pointBM;
            }
            else if (best == distMP)
            {
                pointA = pointAM;
                pointB = pointBP;
            }
            else if (best == distMM)
            {
                pointA = pointAM;
                pointB = pointBM;
            }
        }

        Line resultLine = new Line(pointA, pointB);

        return resultLine;
    }
}

public class Boolean
{
    public bool value;
}