import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Result } from "../../Interfaces/Result";
import { Observable } from "rxjs";
import { SpecializationTranslation } from "../../Interfaces/SpecializationTranslation";
import { SpecializationWithDoctorCount } from "../../Interfaces/SpecializationWithDoctorCount";
import { SpecializationWithTranslations } from "../../Interfaces/SpecializationWithTranslations";
import { environment } from "../../../../environments/environment";

@Injectable({ providedIn: 'root' })
export class SpecializationApiService {
  private baseUrl = environment.ApiBaseUrl + '/Specialization';

  constructor(private http: HttpClient) {}

  // Map short lang to backend format
  private getFormattedLanguage(): string {
    const lang = localStorage.getItem('lang') ?? 'en';
    return lang === 'ar' ? 'ar-EG' : 'en-US';
  }

  getCount(): Observable<Result<number>> {
    return this.http.get<Result<number>>(`${this.baseUrl}/count`);
  }

  getAllWithTranslations(): Observable<Result<SpecializationWithTranslations[]>> {
    return this.http.get<Result<SpecializationWithTranslations[]>>(`${this.baseUrl}/all-with-translations`);
  }

  getWithDoctorCount(): Observable<Result<SpecializationWithDoctorCount[]>> {
    const language = this.getFormattedLanguage();
    const params = new HttpParams().set('language', language);

    return this.http.get<Result<SpecializationWithDoctorCount[]>>(`${this.baseUrl}/with-doctor-count`, { params });
  }

  getTranslation(specializationId: number): Observable<Result<SpecializationTranslation>> {
    const language = this.getFormattedLanguage();
    return this.http.get<Result<SpecializationTranslation>>(
      `${this.baseUrl}/${specializationId}/translation/${language}`
    );
  }

  create(command: SpecializationWithTranslations): Observable<Result<number>> {
    return this.http.post<Result<number>>(this.baseUrl, command);
  }

  updateTranslation(id: number, command: SpecializationTranslation): Observable<Result<any>> {
    return this.http.put<Result<any>>(`${this.baseUrl}/${id}/translations`, command);
  }

  delete(id: number): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.baseUrl}/${id}`);
  }
}
