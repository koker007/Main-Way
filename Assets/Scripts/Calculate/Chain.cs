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

    //Связать 2 звена между собой
    public static void Bind(Chain<Type> back, Chain<Type> next) {
        if(back != null)
            back.next = next; //Предыдущее звено ссылается на следующее

        if(next != null)
            next.back = back; //Следующее ссылается на предыдущее
    }

    //поменять звенья местами
    public static void Swap(Chain<Type> first, Chain<Type> second) {
        Chain<Type> bufferNext;
        Chain<Type> bufferBack;

        //Если ячейки связанны порядком вперед
        if (first.next == second || first == second.back)
        {
            //запоминаем начало и конец разрыва
            bufferBack = first.back;
            bufferNext = second.next;

            //Связываем друг с другом наоборот
            second.next = first;
            first.back = second;

            //связываем начало
            second.back = bufferBack;
            if (bufferBack != null)
                bufferBack.next = second;

            //связываем конец
            first.next = bufferNext;
            if (bufferNext != null)
                bufferNext.back = first;
        }
        //Если ячейки связанны порядном назад
        else if (second.next == first || second == first.back)
        {
            //запоминаем начало и конец разрыва
            bufferBack = second.back;
            bufferNext = first.next;

            //Связываем друг с другом наоборот
            second.back = first;
            first.next = second;

            //связываем начало
            first.back = bufferBack;
            if(bufferBack != null)
                bufferBack.next = first;

            //связываем конец
            second.next = bufferNext;
            if(bufferNext != null)
                bufferNext.back = second;
        }
        //Обычная смена местами
        else {
            bufferBack = first.back;
            bufferNext = second.back;
            Chain<Type> bufferBackEnd = first.next;
            Chain<Type> bufferNextEnd = second.next;

            //Ставим первый объект на место второго
            first.back = bufferNext;
            bufferNext.next = first;
            first.next = bufferNextEnd;
            bufferNextEnd.back = first;

            //Ставим второй на место первого
            second.back = bufferBack;
            bufferBack.next = second;
            second.next = bufferBackEnd;
            bufferNextEnd.back = second;
        }


    }
}
