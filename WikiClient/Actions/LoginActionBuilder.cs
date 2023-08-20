namespace WikiClient.Actions;

public sealed class LoginActionBuilder
{
    private string _username;
    private string _password;
    private string _tokan;

    public LoginActionBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public LoginActionBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public LoginActionBuilder WithToken(string token)
    {
        _tokan = token;
        return this;
    }
    
    public string Build()
    {
        return $"action=login&format=json&lgname={_username}&lgpassword={_password}&lgtoken={_tokan}";
    }
}