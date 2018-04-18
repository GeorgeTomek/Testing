using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class LoginObject
{
    public string message;
    public User user;
    public string login_code;
    public string token;
}
[Serializable]
public class User
{
    public string createdAt;
    public string updatedAt;
    public string id;
    public string email;
    public int failed_login_attempts;
    public bool account_locket;
    public bool active;
    public bool temp_password;
    public string first_name;
    public string last_name;
    public bool is_tandem_recording_enabled;
    public bool is_tandem_annotation_enabled;
    public string domain;
    public string role;
    public string organization;
}

