

namespace DemoAppS7andOPCUA;

public class UAClientHelperAPI
{
    #region Contructor
    public UAClientHelperAPI()
    {
        _applicationConfiguration = CreateClientConfiguration();
    }

    private static UaApplicationConfiguration CreateClientConfiguration()
    {
        // The application configuration can be loaded from any file.
        // ApplicationConfiguration.Load() method loads configuration by looking up a file path in the App.config.
        // This approach allows applications to share configuration files and to update them.
        // This example creates a minimum ApplicationConfiguration using its default constructor.
        UaApplicationConfiguration configuration = new UaApplicationConfiguration();

        // Step 1 - Specify the client identity.
        configuration.ApplicationName = "UA Client Bai 5";
        configuration.ApplicationType = ApplicationType.Client;
        configuration.ApplicationUri = "urn:MyClient"; //Kepp this syntax
        configuration.ProductUri = "Sistrain.DuongQuocGia";

        // Step 2 - Specify the client's application instance certificate.
        // Application instance certificates must be placed in a windows certficate store because that is 
        // the best way to protect the private key. Certificates in a store are identified with 4 parameters:
        // StoreLocation, StoreName, SubjectName and Thumbprint.
        // When using StoreType = Directory you need to have the opc.ua.certificategenerator.exe installed on your machine

        configuration.SecurityConfiguration = new SecurityConfiguration();
        configuration.SecurityConfiguration.ApplicationCertificate = new CertificateIdentifier();
        configuration.SecurityConfiguration.ApplicationCertificate.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.ApplicationCertificate.StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\own";
        configuration.SecurityConfiguration.ApplicationCertificate.SubjectName = configuration.ApplicationName;

        // Define trusted root store for server certificate checks
        configuration.SecurityConfiguration.TrustedIssuerCertificates.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.TrustedIssuerCertificates.StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\issuer";

        configuration.SecurityConfiguration.TrustedPeerCertificates.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.TrustedPeerCertificates.StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\trusted";

        configuration.SecurityConfiguration.RejectedCertificateStore.StoreType = CertificateStoreType.Directory;
        configuration.SecurityConfiguration.RejectedCertificateStore.StorePath = "%CommonApplicationData%\\OPC Foundation\\pki\\rejected";

        configuration.SecurityConfiguration.AutoAcceptUntrustedCertificates = false;



        // Step 3 - Specify the supported transport quotas.
        // The transport quotas are used to set limits on the contents of messages and are
        // used to protect against DOS attacks and rogue clients. They should be set to
        // reasonable values.
        configuration.TransportQuotas = new TransportQuotas();
        configuration.TransportQuotas.OperationTimeout = 600000;
        configuration.TransportQuotas.SecurityTokenLifetime = 3600000;
        configuration.TransportQuotas.MaxStringLength = 1048576;
        configuration.TransportQuotas.MaxByteStringLength = 1048576; //Needed, i.e. for large TypeDictionarys
        configuration.TransportQuotas.MaxArrayLength = 65535;
        configuration.TransportQuotas.MaxMessageSize = 4194304;
        configuration.TransportQuotas.MaxBufferSize = 65535;
        configuration.TransportQuotas.ChannelLifetime = 300000;

        // Step 4 - Specify the client specific configuration.
        configuration.ClientConfiguration = new ClientConfiguration();
        configuration.ClientConfiguration.DefaultSessionTimeout = 360000;
        configuration.ClientConfiguration.MinSubscriptionLifetime = 10000;

        // Step 5 - Validate the configuration.
        // This step checks if the configuration is consistent and assigns a few internal variables
        // that are used by the SDK. This is called automatically if the configuration is loaded from
        // a file using the ApplicationConfiguration.Load() method.
        configuration.Validate(ApplicationType.Client);

        return configuration;
        
    }
    #endregion

    #region field and properties
    private UaApplicationConfiguration _applicationConfiguration = null;

    private Session _session= null;
    public Session Session
    {
        get { return _session; }
    }

    public string ServerUrl { get; set; }

    private Action<IList, IList> _validateResponse;
    #endregion

    #region methods
    public async Task<bool> ConnectAsync()
    {
        try
        {
            bool result;
            if (_session != null && _session.Connected == true)
            { result = true; }
            else
            {
                EndpointDescription endpointDescription = CoreClientUtils.SelectEndpoint(ServerUrl, false);

                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(_applicationConfiguration);
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                Session session = await Session.Create(
                    _applicationConfiguration,
                    endpoint,
                    false,
                    false,
                    _applicationConfiguration.ApplicationName,
                    30 * 60 * 1000,
                    new UserIdentity(),
                    null
                );

                if (session != null && session.Connected == true)
                { _session = session; }
                result = true;
            }
            return result;
        }
        catch
        { return false; }
    }

    public void Disconnect()
    {
        try
        {
            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }    
        }
        catch { }
    }

    public void WriteBoolValue(string nodeId, bool value)
    {
        if (_session == null || _session.Connected == false)
            return;

        try
        {
            WriteValueCollection nodesToWrite = new WriteValueCollection(); // Write the configured nodes

            WriteValue Item = new WriteValue();
            Item.NodeId = new NodeId(nodeId);
            Item.AttributeId = Attributes.Value;
            Item.Value = new DataValue();
            Item.Value.Value = value;
            nodesToWrite.Add(Item);

            StatusCodeCollection results = null;    // Write the node attributes
            DiagnosticInfoCollection diagnosticInfos;

            _session.Write(null,
                            nodesToWrite,
                            out results,
                            out diagnosticInfos);   // Call Write Service

            ClientBase.ValidateResponse(results, nodesToWrite); // Validate the response
        }
        catch { }
    }

    public void WriteInt32Value(string nodeId, Int32 value)
    {
        if (_session == null || _session.Connected == false)
            return;

        try
        {
            WriteValueCollection nodesToWrite = new WriteValueCollection();

            WriteValue Item = new WriteValue();
            Item.NodeId = new NodeId(nodeId);
            Item.AttributeId = Attributes.Value;
            Item.Value = new DataValue();
            Item.Value.Value = (Int32)value;
            nodesToWrite.Add(Item);

            StatusCodeCollection results = null;
            DiagnosticInfoCollection diagnosticInfos;

            _session.Write(null,
                            nodesToWrite,
                            out results,
                            out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToWrite);
        }
        catch { }
    }

    public void WriteInt16Value(string nodeId, Int16 value)
    {
        if (_session == null || _session.Connected == false)
            return;

        try
        {
            WriteValueCollection nodesToWrite = new WriteValueCollection();

            WriteValue Item = new WriteValue();
            Item.NodeId = new NodeId(nodeId);
            Item.AttributeId = Attributes.Value;
            Item.Value = new DataValue();
            Item.Value.Value = (Int16)value;
            nodesToWrite.Add(Item);

            StatusCodeCollection results = null;
            DiagnosticInfoCollection diagnosticInfos;

            _session.Write(null,
                            nodesToWrite,
                            out results,
                            out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToWrite);
        }
        catch { }
    }

    public void WriteFloatValue(string nodeId, float value)
    {
        if (_session == null || _session.Connected == false)
            return;

        try
        {
            WriteValueCollection nodesToWrite = new WriteValueCollection();

            WriteValue Item = new WriteValue();
            Item.NodeId = new NodeId(nodeId);
            Item.AttributeId = Attributes.Value;
            Item.Value = new DataValue();
            Item.Value.Value = (float)value;
            nodesToWrite.Add(Item);

            StatusCodeCollection results = null;
            DiagnosticInfoCollection diagnosticInfos;

            _session.Write(null,
                            nodesToWrite,
                            out results,
                            out diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToWrite);
        }
        catch { }
    }
    #endregion
}
