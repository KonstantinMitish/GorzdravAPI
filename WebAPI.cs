using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace GorzdravAPI
{
  public class WebAPI
  {
    // Web handle
    private const string ApiUrl = "https://gorzdrav.spb.ru/api/";
    private CookieContainer Cookies = new CookieContainer();
    //private string Sessia = "9geujzpwnab19yzfanr3w1s88eje3pg4";
    struct POSTData
    {
      public string Key, Val;
      public POSTData( string k, string v )
      {
        Key = k;
        Val = v;
      }
    }
    private string POST(string Method, POSTData[] Data)
    {
      try
      {
        HttpWebRequest request = (HttpWebRequest) WebRequest.Create(ApiUrl + Method + "/");
        request.ContentType = "application/x-www-form-urlencoded";
        request.Headers["X-Requested-With"] = "XMLHttpRequest";
        request.CookieContainer = Cookies;
        request.Method = "POST";
        string p = "";
        for (int i = 0; i < Data.Length; i++)
        {
          p += WebUtility.UrlEncode(Data[i].Key) + "=" + WebUtility.UrlEncode(Data[i].Val);
          if (i != Data.Length - 1)
            p += "&";
        }
        byte[] postBytes = Encoding.UTF8.GetBytes(p);
        Stream postDataStream = request.GetRequestStream();
        postDataStream.Write(postBytes, 0, postBytes.Length);
        postDataStream.Close();
        var response = (HttpWebResponse) request.GetResponse();
        Cookies.Add(response.Cookies);
        var responseDataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseDataStream);
        string responseFromServer = reader.ReadToEnd();
        reader.Close();
        responseDataStream.Close();
        response.Close();
        return responseFromServer;
      }
      catch( Exception e )
      {
        return "{" + 
          "\"response\": {}," +
          " \"success\": false," +
          " \"error\": " +
          "{"+
            "\"ErrorDescription\": \"" + e.Message + "\", " +
            "\"IdError\": 239" +
          "}" +
        "}";
      }
    }

    // Common types
    public class Error
    {
      public string ErrorDescription { get; set; }
      public int IdError { get; set; }
    }
    public class Date
    {
      public int year { get; set; }
      public string day_verbose { get; set; }

      public int month { get; set; }
      public string month_verbose { get; set; }
      public string time { get; set; }
      public DateTime iso { get; set; }
      public int day { get; set; }
    }

    // API methods
    public class CheckPatientResponse
    {
      public class CheckPatientResponseResponse
      {
        public string phone { get; set; }
        public string history_id { get; set; }
        public string email { get; set; }
        public string patientAriaNumber { get; set; }
        public string patient_id { get; set; }
      }

      public CheckPatientResponseResponse response { get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }
    public CheckPatientResponse CheckPatient(string Name, string Surname, string Fathername, string EMail, string Phone, DateTime Birthday, int ClinicId)
    {
      try
      {
        return JsonConvert.DeserializeObject<CheckPatientResponse>(POST("check_patient",
          new[]
          {
            new POSTData("patient_form-first_name", Name),
            new POSTData("patient_form-last_name", Surname),
            new POSTData("patient_form-middle_name", Fathername),
            new POSTData("patient_form-insurance_series", ""),
            new POSTData("patient_form-insurance_number", ""),
            new POSTData("patient_form-email", EMail),
            new POSTData("patient_form-phone", Phone),
            new POSTData("patient_form-birthday", Birthday.ToString("yyyy-MM-dd") + "T00:00:00.000Z"),
            new POSTData("patient_form-clinic_id", ClinicId.ToString()),
            new POSTData("csrfmiddlewaretoken", "NOTPROVIDED")
          }));
      }
      catch
      {
        return new CheckPatientResponse()
        {
          success = false
        };
      }
    }

    public class GetDistrictsResponse
    {
      public class District
      {
        public string Name { get; set; }
        public int Id { get; set; }
      }
      public District[] districts{ get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }
    public GetDistrictsResponse GetDistricts()
    {
      return new GetDistrictsResponse()
      {
        districts = new []
        {
          new GetDistrictsResponse.District{Id = 1, Name = "Адмиралтейский"},
          new GetDistrictsResponse.District{Id = 2, Name = "Василеостровский"},
          new GetDistrictsResponse.District{Id = 3, Name = "Выборгский"},
          new GetDistrictsResponse.District{Id = 4, Name = "Калининский"},
          new GetDistrictsResponse.District{Id = 5, Name = "Кировский"},
          new GetDistrictsResponse.District{Id = 6, Name = "Колпинский"},
          new GetDistrictsResponse.District{Id = 7, Name = "Красногвардейский"},
          new GetDistrictsResponse.District{Id = 8, Name = "Красносельский"},
          new GetDistrictsResponse.District{Id = 9, Name = "Кронштадтский"},
          new GetDistrictsResponse.District{Id = 10, Name = "Курортный"},
          new GetDistrictsResponse.District{Id = 11, Name = "Московский"},
          new GetDistrictsResponse.District{Id = 12, Name = "Невский"},
          new GetDistrictsResponse.District{Id = 13, Name = "Петроградский"},
          new GetDistrictsResponse.District{Id = 14, Name = "Петродворцовый"},
          new GetDistrictsResponse.District{Id = 15, Name = "Приморский"},
          new GetDistrictsResponse.District{Id = 16, Name = "Пушкинский"},
          new GetDistrictsResponse.District{Id = 17, Name = "Фрунзенский"},
          new GetDistrictsResponse.District{Id = 18, Name = "Центральный"}
        },
        error = new Error { ErrorDescription = null, IdError = 0},
        success = true
      };
    }

    public class ClinicListResponse
    {
      public class Clinic
      { 
        public string LPUType { get;set; }
        public string LpuName { get; set; }
        public string District { get; set; }
        public string WorkTime { get; set; }
        public bool IsEnableAppointment { get; set; }
        public bool InfoStand { get; set; }
        public string LpuType { get; set; }
        public bool IsEnableInternet { get; set; }
        public string InternetReference { get; set; }
        public int IdLPU { get; set; }
        public string PhoneCallCentre { get; set; }
        public string LPUShortName { get; set; }
        public string PartOf { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public int IdDistrict { get; set; }
        public int ExternalLpuId { get; set; }
        public string ExternalHubId { get; set; }
        public string ExternalGisId { get; set; }
        public string Oid { get; set; }
        public string LPUFullName { get; set; }
        public string email { get; set; }
        public string Address { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
      }

      public Clinic[] response { get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }
    public ClinicListResponse ClinicList( int DistrictId )
    {
      try 
        { 
      return JsonConvert.DeserializeObject<ClinicListResponse>(POST("clinic_list",
        new[]
        {
          new POSTData("district_form-district_id", DistrictId.ToString())
        }));
      }
      catch
      {
        return new ClinicListResponse()
        {
          success = false
        };
      }
    }
    
    public class CheckClinicResponse
    {
      public string changes { get; set; }
      public class Spesiality
      {
        public string NameSpesiality { get; set; }

        public int FerIdSpesiality { get; set; }
        public int IdSpesiality { get; set; }
        public int CountFreeTicket { get; set; }
        public Date LastDate { get; set; }
        public Date NearestDate { get; set; }
        public int CountFreeParticipantIE { get; set; }
      }
      public Spesiality[] response { get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }
    public CheckClinicResponse CheckClinic(int ClinicId)
    {
      try
      {
        return JsonConvert.DeserializeObject<CheckClinicResponse>(POST("check_clinic",
          new[]
          {
            new POSTData("clinic_form-clinic_id", ClinicId.ToString()),
            new POSTData("clinic_form-patient_id", ""),
            new POSTData("clinic_form-history_id", "")
          }));
      }
      catch
      {
        return new CheckClinicResponse()
        {
          success = false
        };
      }
    }

    public class DoctorListResponse
    {
      public class Doctor
      {
        public string Comment { get; set; }
        public string AriaNumber { get; set; }
        public string Name { get; set; }
        public string IdDoc { get; set; }
        public int CountFreeTicket { get; set; }
        public Date LastDate { get; set; }
        public string WorkingTime { get; set; }
        public Date NearestDate { get; set; }
        public int CountFreeParticipantIE { get; set; }
      }
      public Doctor[] response { get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }
    public DoctorListResponse DoctorList(int ClinicId, int SpecialityId)
    {
      try
      {
        return JsonConvert.DeserializeObject<DoctorListResponse>(POST("doctor_list",
          new[]
          {
            new POSTData("speciality_form-speciality_id", SpecialityId.ToString()),
            new POSTData("speciality_form-clinic_id", ClinicId.ToString()),
            new POSTData("speciality_form-patient_id", ""),
            new POSTData("speciality_form-history_id", "")
          }));
      }
      catch
      {
        return new DoctorListResponse()
        {
          success = false
        };
      }
    }

    public class AppointmentListResponse
    {
      public class Appointment
      {
        public Date date_end { get; set; }
        public Date date_start { get; set; }
        public string room { get;set; }
        public string id { get; set; }
        public string address { get; set; }
      }
      public Dictionary<string, Appointment[]> response { get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }
    public AppointmentListResponse AppointmentList(int ClinicId, string DoctorId)
    {
      try
      {
        return JsonConvert.DeserializeObject<AppointmentListResponse>(POST("appointment_list",
          new[]
          {
            new POSTData("doctor_form-doctor_id", DoctorId),
            new POSTData("doctor_form-clinic_id", ClinicId.ToString()),
            new POSTData("doctor_form-patient_id", ""),
            new POSTData("doctor_form-history_id", ""),
            new POSTData("doctor_form-appointment_type", "")
          }));
      }
      catch
      {
        return new AppointmentListResponse()
        {
          success = false
        };
      }
    }


    public class SignupResponse
    {
      public string response { get; set; }
      public bool success { get; set; }
      public Error error { get; set; }
    }

    public class SignupResponseTemp
    {
      public bool success { get; set; }
      public Error error { get; set; }
    }

    public SignupResponse Signup(int ClinicId, string PatientId, string AppointmentId, string Phone, string EMail, string VisitStart)
    {
      try
      {
        string resp = POST("signup",
          new[]
          {
            new POSTData("csrfmiddlewaretoken", "NOTPROVIDED"),
            new POSTData("appointment_form-clinic_id", ClinicId.ToString()),
            new POSTData("appointment_form-history_id", ""),
            new POSTData("appointment_form-patient_id", PatientId),
            new POSTData("appointment_form-appointment_id", AppointmentId),
            new POSTData("appointment_form-phone", Phone),
            new POSTData("appointment_form-email", EMail),
            new POSTData("appointment_form-visit_start", VisitStart),
            new POSTData("appointment_form-referal_id", "")
          });
        SignupResponseTemp t = JsonConvert.DeserializeObject<SignupResponseTemp>(resp);
        SignupResponse r = new SignupResponse()
        {
          error = t.error,
          success = t.success,
          response = resp
        };
        return r;
      }
      catch
      {
        return new SignupResponse(){success = false};
      }
    }
  }
}
