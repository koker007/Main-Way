using System.Collections;
using System.Collections.Generic;

public class Chain<Type>
{
    public Chain<Type> back;
    public Type data {
        get {
            return Data;
        }
    }
    public Chain<Type> next;

    Type Data;

    public Chain(Type Data){
        this.Data = Data;
    }

    //������� 2 ����� ����� �����
    public static void Bind(Chain<Type> back, Chain<Type> next) {
        if(back != null)
            back.next = next; //���������� ����� ��������� �� ���������

        if(next != null)
            next.back = back; //��������� ��������� �� ����������
    }

    //�������� ������ �������
    public static void Swap(Chain<Type> first, Chain<Type> second) {
        Chain<Type> bufferNext;
        Chain<Type> bufferBack;

        //���� ������ �������� �������� ������
        if (first.next == second || first == second.back)
        {
            //���������� ������ � ����� �������
            bufferBack = first.back;
            bufferNext = second.next;

            //��������� ���� � ������ ��������
            second.next = first;
            first.back = second;

            //��������� ������
            second.back = bufferBack;
            if (bufferBack != null)
                bufferBack.next = second;

            //��������� �����
            first.next = bufferNext;
            if (bufferNext != null)
                bufferNext.back = first;
        }
        //���� ������ �������� �������� �����
        else if (second.next == first || second == first.back)
        {
            //���������� ������ � ����� �������
            bufferBack = second.back;
            bufferNext = first.next;

            //��������� ���� � ������ ��������
            second.back = first;
            first.next = second;

            //��������� ������
            first.back = bufferBack;
            if(bufferBack != null)
                bufferBack.next = first;

            //��������� �����
            second.next = bufferNext;
            if(bufferNext != null)
                bufferNext.back = second;
        }
        //������� ����� �������
        else {
            bufferBack = first.back;
            bufferNext = second.back;
            Chain<Type> bufferBackEnd = first.next;
            Chain<Type> bufferNextEnd = second.next;

            //������ ������ ������ �� ����� �������
            first.back = bufferNext;
            bufferNext.next = first;
            first.next = bufferNextEnd;
            bufferNextEnd.back = first;

            //������ ������ �� ����� �������
            second.back = bufferBack;
            bufferBack.next = second;
            second.next = bufferBackEnd;
            bufferNextEnd.back = second;
        }


    }
}
