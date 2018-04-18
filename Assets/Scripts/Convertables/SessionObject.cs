using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SessionObject
{
    public string createdAt;
    public string updatedAt;
    public string id;
    public string session_id;
    public string call_type;
    public CallRecord[] call_record;
    public string initiator_id;
    public string initiator_name;
    public string initiator_email;
    public string[] viewers;
    public double duration;
    public string duration_stream_created;
    public bool active;
    public string screenshots;
    public string video_archive_id;
    public string video_archive_name;
    public bool video_archive_active;
    public bool video_archive_processed;
    public string video_archive_user;
    public string shared_with_ids;
    public string shared_with;
    public string organization;
    public string call_status;
    public string session_token;
}

[Serializable]
public class CallRecord
{
    public string status;
    public string by_id;
    public string by_name;
    public string device;
    public string device_id;
    public string ip_address;
    public string created;
}