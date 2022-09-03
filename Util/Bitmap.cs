using System;
using System.Collections;
using System.Collections.Generic;

public class Bitmap
{
    int m_bitlength;
    byte[] data;

    public Bitmap(byte[] data)
    {
        this.data = data;
        m_bitlength = data.Length - 2 + data[0];
    }
    public Bitmap(int bitlength)
    {
        m_bitlength = bitlength;
        data = new byte[m_bitlength / 8 + 2];
        data[0] = (byte)(m_bitlength % 8);
    }

    public bool getBit(int index)
    {
        int cell = index / 8 + 1;
        int offset = index % 8;
        return (data[cell] & (1 << offset)) > 0;
    }

    public void setBit(int index, bool flag)
    {
        int cell = index / 8 + 1;
        int offset = index % 8;
        if (flag)
        {
            data[cell] = (byte)(data[cell] | (1 << offset));
        }
        else
        {
            data[cell] = (byte)(data[cell] & ~(1 << offset));
        }
    }

    public int BitLength()
    {
        return m_bitlength;
    }
    public int ByteLength()
    {
        return data.Length;
    }
    public byte[] getData()
    {
        return data;
    }

}
