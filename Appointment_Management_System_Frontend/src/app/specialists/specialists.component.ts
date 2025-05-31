import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { RippleModule } from 'primeng/ripple';
import { AsyncPipe } from '@angular/common';
import { SpecializationTranslation } from '../core/Interfaces/SpecializationTranslation';
import { SpecializationStateService } from '../core/services/State/specialization-state.service';
import { SpecializationWithTranslations } from '../core/Interfaces/SpecializationWithTranslations';
import { AddSpecialistComponent } from './add-specialist/add-specialist.component';
import { UpdateSpecialistTranslationComponent } from './update-specialist-translation/update-specialist-translation.component';

@Component({
  selector: 'app-specialists',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    DialogModule,
    ConfirmDialogModule,
    RippleModule,
    AsyncPipe
  ],
  templateUrl: './specialists.component.html',
  styleUrl: './specialists.component.css',
  providers: [DialogService, ConfirmationService]
})
export class SpecialistsComponent implements OnInit {
  
  private state = inject(SpecializationStateService);
  private dialogService = inject(DialogService);
  private confirmService = inject(ConfirmationService);
  private messageService = inject(MessageService);
  ref: DynamicDialogRef | null = null;

  specializations$ = this.state.specializations;
  expandedSpecializationId: number | null = null;
  expandedOfficeId: number | null = null;

  ngOnInit(): void {
    this.state.loadAllSpecializations();
  }

  toggleExpansion(id: number): void {
    this.expandedOfficeId = this.expandedOfficeId === id ? null : id;
  }

  getTranslation(specialization: SpecializationWithTranslations, lang: string): SpecializationTranslation | null {
    return specialization.translations.find(t => t.language === lang) || null;
  }

  openAddSpecializationDialog() {
    this.ref = this.dialogService.open(AddSpecialistComponent, {
      width: '50%',
      showHeader: false,
      closable: true,
    });
  }

  confirmDeleteSpecialization(id: number) {
    this.confirmService.confirm({
      message: 'Are you sure you want to delete this specialization?',
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'btn-outline-danger me-2',
      rejectButtonStyleClass: 'btn-outline-primary me-2',
      accept: () => {
        this.state.deleteSpecialization(id);
        // this.messageService.add({
        //   severity: 'success',
        //   summary: 'Success',
        //   detail: 'Specialization deleted'
        // });
      }
    });
  }


  openEditTranslationDialog(translation: SpecializationTranslation, specializationId: number) {
    this.ref = this.dialogService.open(UpdateSpecialistTranslationComponent, {
         width: '50%',
         showHeader: false,
         closable: true,
         data: translation,
      });
  }

}
