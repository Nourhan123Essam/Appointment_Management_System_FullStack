import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Doctor } from '../../Interfaces/Doctor';
import { Observable } from 'rxjs';
import { DoctorQualification } from '../../Interfaces/DoctorQualification';
import { DoctorAvailability } from '../../Interfaces/DoctorAvailability';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DoctorApiService {

  private baseUrl = `${environment.ApiBaseUrl}/Doctor`;

  constructor(private http: HttpClient) {}

  // ================================
  // Doctor CRUD Operations
  // ================================

  fetchDoctors(): Observable<Doctor[]>{
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctorsBasicData`);
  }

  addDoctor(doctor: Partial<Doctor>): Observable<Doctor> {
    return this.http.post<Doctor>(`${this.baseUrl}`, doctor);
  }

  getAllDoctors(): Observable<Doctor[]> {
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctors`);
  }

  updateDoctor(doctorId: string, doctor: Partial<Doctor>): Observable<string> {
    return this.http.put<string>(`${this.baseUrl}/${doctorId}`, doctor);
  }

  deleteDoctor(doctorId: string): Observable<string> {
    return this.http.delete<string>(`${this.baseUrl}/${doctorId}`);
  }

  // ================================
  // Qualifications CRUD Operations
  // ================================
 
  fetchDoctorQualifications(doctorId: string): Observable<DoctorQualification[]> {
    return this.http.get<DoctorQualification[]>(`${this.baseUrl}/GetQualificationByDoctorId?doctorId=${doctorId}`);
  }

  getQualificationById(id: number): Observable<DoctorQualification> {
    return this.http.get<DoctorQualification>(`${this.baseUrl}/GetQualificationById/${id}`);
  }

  addDoctorQualification(qualification: Partial<DoctorQualification>): Observable<DoctorQualification> {
    return this.http.post<DoctorQualification>(`${this.baseUrl}/CreateQualification`, qualification);
  }

  updateDoctorQualification(id: number, qualification: Partial<DoctorQualification>): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/UpdateQualification/${id}`, qualification);
  }

  deleteDoctorQualification(id: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/DeleteQualification/${id}`);
  }

  // ================================
  // Availability CRUD Operations
  // ================================

  fetchDoctorAvailabilities(doctorId: string): Observable<DoctorAvailability[]> {
    return this.http.get<DoctorAvailability[]>(`${this.baseUrl}/GetAvailabilityByDoctorId/${doctorId}`);
  }

  getAvailabilityById(id: number): Observable<DoctorAvailability> {
    return this.http.get<DoctorAvailability>(`${this.baseUrl}/GetAvailabilityById/${id}`);
  }

  addDoctorAvailability(availability: Partial<DoctorAvailability>): Observable<DoctorAvailability> {
    return this.http.post<DoctorAvailability>(`${this.baseUrl}/CreateAvailability`, availability);
  }

  updateDoctorAvailability(id: number, availability: Partial<DoctorAvailability>): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/UpdateAvailability/${id}`, availability);
  }

  deleteDoctorAvailability(id: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/DeleteAvailability/${id}`);
  }
}
