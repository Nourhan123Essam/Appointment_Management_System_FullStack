import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment.development';

export interface PatientDto {
  id: string;
  fullName: string;
  email: string;
  gender: string;
  dateOfBirth: Date;
  address: string;
  profilePictureUrl: string;
  createdAt: string;
  appointmentCount: number;
}

export interface AppointmentDto {
  id: string;
  appointmentTime: string;
  doctorName: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface PatientQueryParams {
  pageNumber: number;
  pageSize: number;
  search?: string;
  sortBy?: string;
  isDescending?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class PatientApiService {

  private readonly apiUrl =  `${environment.ApiBaseUrl}/Patient`;

  constructor(private http: HttpClient) {}

  getPatients(params: PatientQueryParams): Observable<PagedResult<PatientDto>> {
    // debugger;
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber)
      .set('pageSize', params.pageSize)
      .set('search', params.search || '')
      .set('sortBy', params.sortBy || '')
      .set('isDescending', params.isDescending?.toString() || 'false');
  
    return this.http.get<PagedResult<PatientDto>>(this.apiUrl, { params: httpParams });
  }

  getPatientById(id: string): Observable<PatientDto> {
    return this.http.get<PatientDto>(`${this.apiUrl}/${id}`);
  }

  deletePatient(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getPatientAppointments(id: string): Observable<AppointmentDto[]> {
    return this.http.get<AppointmentDto[]>(`${this.apiUrl}/${id}/appointments`);
  }
}
