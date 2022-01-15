namespace DemoAppS7andOPCUA;

public class S7Service
{
    private readonly Plc _plc;
    public string Address { get; set; }

    public S7Service(string address)
    {
        _plc = new Plc(CpuType.S7200, address, 0, 1);
        Address = address;
    }

    public bool Connect()
    {
        try
        {
            _plc.Open();
            bool result = _plc.IsConnected;

            return result;
        }
        catch
        { return false; }
    }

    public bool ReConnect()
    {
        try
        {
            if (_plc.IsConnected)
            { return true; }
            else
            {
                bool result = Connect();
                
                return result;
            }
        }
        catch
        { return false; }
    }

    public void Disconnect()
    {
        try { _plc.Close(); }
        catch { }
    }

    public bool ReadBool(int db, int startbyte, int index)
    {
        byte[] byteArray = _plc.ReadBytes(DataType.DataBlock, db, startbyte, 1);
        
        return byteArray[0].SelectBit(index);
    }

    public object Read(string variable)
    {
        var result = _plc.Read(variable);

        if (result == null)
        { return new object(); }
        else
        { return result; }
    }
}
