import { Injectable } from "@angular/core";
import { OfficeTranslation } from "../../Interfaces/OfficeTranslation";
import { BehaviorSubject, catchError, map, Observable, of, tap } from "rxjs";
import { Office } from "../../Interfaces/Office";
import { OfficeApiService } from "../Api/office-api.service";

// services/office-state.service.ts
@Injectable({ providedIn: 'root' })
export class OfficeStateService {
  private officesSubject = new BehaviorSubject<Office[]>([]);
  offices$ = this.officesSubject.asObservable();

  constructor(private api: OfficeApiService) {}

  loadOffices(): void {
    this.api.getAllOffices().subscribe(data => {
        console.log("offices", data);
        
      this.officesSubject.next(data);
    });
  }

  createOffice(command: Office): Observable<boolean> {
    return this.api.createOffice(command).pipe(
      tap(res => {
        if (res.succeeded) {
          // append the new office
          console.log("Created office", res);
          
          const current = this.officesSubject.value;
          this.officesSubject.next([...current, res.data]);
        }
      }),
      map(res => res.succeeded),
      catchError(() => of(false))
    );
  }


  deleteOffice(id: number): void {
    this.api.deleteOffice(id).subscribe(() => {
      const current = this.officesSubject.getValue();
      this.officesSubject.next(current.filter(o => o.id !== id));
    });
  }

  updateTranslation(translation: OfficeTranslation): Observable<boolean> {
    return this.api.updateTranslation(translation.officeId, translation).pipe(
        map(() => {
        const updatedOffices = this.officesSubject.getValue().map(office => {
            if (office.translations.some(t => t.id === translation.id)) {
            return {
                ...office,
                translations: office.translations.map(t =>
                t.id === translation.id ? translation : t
                )
            };
            }
            return office;
        });

        this.officesSubject.next(updatedOffices);
        return true; // success
        }),
        catchError((err) => {
        console.error('Update translation failed', err);
        return of(false); // failed
        })
    );
    }

}
