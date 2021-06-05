#!/usr/bin/env python
# coding: utf-8

# In[24]:


import urllib3
import json
from hashlib import sha256
import time


# In[ ]:





# In[25]:


mobile="Mobile No"
key="U2FsdGVkX18vDwDor+oOIG7vSUnINtlc/pxQcNiBulCm8LT5Sza+aIISKLqImbpMnRYgsN2QACPhggLWgZEpQg=="
date="06-06-2021"
district="770"
age=18


# In[ ]:





# In[26]:


header={"accept":"application/json","Accept-Language":"en_US","Origin":"https://selfregistration.cowin.gov.in","Referer":"https://selfregistration.cowin.gov.in","user-agent":"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36"}


# In[27]:


http=urllib3.PoolManager()


# In[28]:


#apis
generate_otp="https://cdn-api.co-vin.in/api/v2/auth/generateMobileOTP"
validate_otp="https://cdn-api.co-vin.in/api/v2/auth/validateMobileOtp"
find_by_dist="https://cdn-api.co-vin.in/api/v2/appointment/sessions/findByDistrict?district_id={0}&date={1}".format(district,date)


# In[ ]:





# In[29]:


enc_data=json.dumps({"mobile":mobile,"secret":key}).encode("utf-8")


# In[ ]:





# In[30]:


r=http.request("POST",generate_otp,headers=header,body=enc_data)


# In[31]:


txnId=json.loads(r.data.decode('utf-8'))["txnId"]


# In[32]:


r.data


# In[ ]:





# In[33]:


otpsha=sha256(str(input()).encode("utf-8")).hexdigest()


# In[ ]:





# In[34]:


enc_data=json.dumps({"otp":otpsha,"txnId":txnId,"secret":key}).encode("utf-8")


# In[35]:


r=http.request("POST",validate_otp,headers=header,body=enc_data)


# In[36]:


token=json.loads(r.data.decode('utf-8'))["token"]


# In[37]:


header["Authorization"]="Bearer "+token


# In[38]:


available=""
tries=1
while True:
    print('#Try: ',tries)
    r=http.request("GET",find_by_dist,headers=header)
    data=json.loads(r.data.decode())
    sessions=data["sessions"]
    #print(sessions)
    if len(sessions)>0:
        for session in sessions:
            print(session["name"],session["session_id"])
            if session["min_age_limit"]==age and session["pincode"] in pincodes and session["available_capacity_dose1"]>0:
                available=session 
                break
    if available!="":
        break
    tries=tries+1
    time.sleep(0.20)
        
#session_id=center["session_id"]
print(available["pincode"])
print(available["name"])

