import { Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Office } from "../../Interfaces/Office";
import { map, Observable } from "rxjs";
import { OfficeTranslation } from "../../Interfaces/OfficeTranslation";

// services/office-api.service.ts
@Injectable({ providedIn: 'root' })
export class OfficeApiService {
  private baseUrl = environment.ApiBaseUrl + '/office';

  constructor(private http: HttpClient) {}

  getAllOffices(): Observable<Office[]> {
    return this.http.get<any>(`${this.baseUrl}/admin/all`)
      .pipe(map(res => res.data));
  }

  createOffice(command: Office): Observable<any> {
  return this.http.post<any>(this.baseUrl, command);
}

  deleteOffice(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  
  updateTranslation(officeId: number, translation: OfficeTranslation): Observable<any> {
  return this.http.put(`${this.baseUrl}/translation`, { officeId, translation });
}

}
