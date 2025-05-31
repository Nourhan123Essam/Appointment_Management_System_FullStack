import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, catchError, finalize, Observable, of, tap } from "rxjs";
import { SpecializationTranslation } from "../../Interfaces/SpecializationTranslation";
import { SpecializationWithDoctorCount } from "../../Interfaces/SpecializationWithDoctorCount";
import { SpecializationWithTranslations } from "../../Interfaces/SpecializationWithTranslations";
import { SpecializationApiService } from "../Api/specialization-api.service";
import { MessageService } from "primeng/api";

@Injectable({ providedIn: 'root' })
export class SpecializationStateService {
  private specializations$ = new BehaviorSubject<SpecializationWithTranslations[]>([]);
  public specializationsWithDoctorCount$ = new BehaviorSubject<SpecializationWithDoctorCount[]>([]);
  private loading$ = new BehaviorSubject<boolean>(false);

  private messageService = inject(MessageService);

  constructor(private api: SpecializationApiService) {}

  readonly specializations = this.specializations$.asObservable();
  readonly specializationsWithDoctorCount = this.specializationsWithDoctorCount$.asObservable();
  readonly loading = this.loading$.asObservable();

  //== Helper to show toast ==
  private toast(type: 'success' | 'error', detail: string): void {
    this.messageService.add({ severity: type, summary: type === 'success' ? 'Success' : 'Error', detail });
  }

  //== Admin Only ==
  loadAllSpecializations(): void {
    this.loading$.next(true);
    this.api.getAllWithTranslations().pipe(
      tap(res => {
        if (res.succeeded) {
          this.specializations$.next(res.data ?? []);
        } else {
          this.toast('error', res.message || 'Failed to load specializations');
        }
      }),
      catchError(() => {
        this.toast('error', 'Unexpected error occurred');
        return of(null);
      }),
      finalize(() => this.loading$.next(false))
    ).subscribe();
  }

  //== Public (home page, etc.) ==
  loadSpecializationsWithDoctorCount(): void {
    this.api.getWithDoctorCount().pipe(
      tap(res => {
        if (res.succeeded) {
          this.specializationsWithDoctorCount$.next(res.data ?? []);
        } else {
          this.toast('error', res.message || 'Failed to load doctor counts');
        }
      }),
      catchError(() => {
        this.toast('error', 'Unexpected error occurred');
        return of(null);
      })
    ).subscribe();
  }

  createSpecialization(dto: SpecializationWithTranslations): void {
    this.api.create(dto).subscribe(res => {
      if (res.succeeded) {
        this.toast('success', res.message || 'Specialization created');
        const updated = [dto, ...this.specializations$.getValue()];
        this.specializations$.next(updated);
      } else {
        this.toast('error', res.message || 'Creation failed');
      }
    });
  }

  deleteSpecialization(id: number): void {
    this.api.delete(id).subscribe(res => {
      if (res.succeeded) {
        this.toast('success', res.message || 'Specialization deleted');
        const filtered = this.specializations$.getValue().filter(s => s.id !== id);
        this.specializations$.next(filtered);
      } else {
        this.toast('error', res.message || 'Deletion failed');
      }
    });
  }

  updateTranslation(id: number, dto: SpecializationTranslation): void {
    this.api.updateTranslation(id, dto).subscribe(res => {
      if (res.succeeded) {
        this.toast('success', res.message || 'Translation updated');
        const current = this.specializations$.getValue().map(s => {
          if (s.id === id) {
            const updatedTranslations = s.translations.map(t =>
              t.language === dto.language ? { ...t, name: dto.name } : t
            );
            return { ...s, translations: updatedTranslations };
          }
          return s;
        });
        this.specializations$.next(current);
      } else {
        this.toast('error', res.message || 'Update failed');
      }
    });
  }
}