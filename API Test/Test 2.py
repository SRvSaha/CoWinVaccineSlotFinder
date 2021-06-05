#!/usr/bin/env python
# coding: utf-8

# In[1]:


import urllib3
import json
from hashlib import sha256
import time


# In[ ]:





# In[2]:


import warnings
warnings.filterwarnings('ignore')


# In[3]:


mobile=""
key="U2FsdGVkX18vDwDor+oOIG7vSUnINtlc/pxQcNiBulCm8LT5Sza+aIISKLqImbpMnRYgsN2QACPhggLWgZEpQg=="
date="06-06-2021"
districts={"770":["380009","380007"]}
age=18


# In[ ]:





# In[4]:


header={"accept":"application/json","Accept-Language":"en_US","Origin":"https://selfregistration.cowin.gov.in","Referer":"https://selfregistration.cowin.gov.in","user-agent":"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36"}


# In[5]:


http=urllib3.PoolManager()


# In[6]:


#apis
generate_otp="https://cdn-api.co-vin.in/api/v2/auth/generateMobileOTP"
validate_otp="https://cdn-api.co-vin.in/api/v2/auth/validateMobileOtp"
find_by_dist="https://cdn-api.co-vin.in/api/v2/appointment/sessions/findByDistrict?district_id={0}&date={1}"
find_by_pin="https://cdn-api.co-vin.in/api/v2/appointment/sessions/calendarByPin?pincode={0}&date={1}"


# In[ ]:





# In[7]:


enc_data=json.dumps({"mobile":mobile,"secret":key}).encode("utf-8")


# In[ ]:





# In[8]:


r=http.request("POST",generate_otp,headers=header,body=enc_data)


# In[9]:


txnId=json.loads(r.data.decode('utf-8'))["txnId"]


# In[10]:


r.data


# In[ ]:





# In[11]:


otpsha=sha256(str(input()).encode("utf-8")).hexdigest()


# In[ ]:





# In[12]:


enc_data=json.dumps({"otp":otpsha,"txnId":txnId,"secret":key}).encode("utf-8")


# In[13]:


r=http.request("POST",validate_otp,headers=header,body=enc_data)


# In[14]:


token=json.loads(r.data.decode('utf-8'))["token"]


# In[15]:


header["Authorization"]="Bearer "+token


# In[16]:


tries=1
response=""


# In[17]:


def find_by_district():
    global tries
    global response
    while True:
        for district in districts.keys():
            print('#Try: ',tries)
            pincodes=districts[district]
            r=http.request("GET",find_by_dist.format(district,date),headers=header, retries=False)
            tries=tries+1
            data=json.loads(r.data.decode())
            response=data
            sessions=data["sessions"]
            #print(sessions)
            if len(sessions)>0:
                for session in sessions:
                    print(session["name"],session["session_id"])
                    if session["min_age_limit"]==age and session["pincode"] in pincodes and session["available_capacity_dose1"]>0:
                        return session
            


# In[18]:


def find_by_pincode():
    global tries
    global response
    while  True:        
        for district in districts.keys():
            for pincode in districts[district]:
                print('#Try: ',tries)
                r=http.request("GET",find_by_pin.format(pincode,date),headers=header, retries=False)
                tries=tries+1
                data=json.loads(r.data.decode())
                response=data
                sessions=data["sessions"]
                #print(sessions)
                if len(sessions)>0:
                    for session in sessions:
                        print(session["name"],session["session_id"])
                        if session["min_age_limit"]==age and session["available_capacity_dose1"]>0:
                            return session


# In[19]:


def available():
    try:
        a=find_by_district()
        return a
    except KeyError:
        return find_by_pincode()


# In[22]:


session=available()


# In[23]:


response


# In[ ]:




