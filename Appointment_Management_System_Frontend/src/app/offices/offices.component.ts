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
import { OfficeStateService } from '../core/services/State/office-state.service';
import { Office } from '../core/Interfaces/Office';
import { OfficeTranslation } from '../core/Interfaces/OfficeTranslation';
import { AddOfficeComponent } from './add-office/add-office.component';
import { UpdateOfficeTranslationComponent } from './update-office-translation/update-office-translation.component';

@Component({
  selector: 'app-offices',
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
  templateUrl: './offices.component.html',
  styleUrl: './offices.component.css',
  providers: [DialogService, ConfirmationService]
})
export class OfficesComponent implements OnInit {
  private state = inject(OfficeStateService);
  private dialogService = inject(DialogService);
  private confirmService = inject(ConfirmationService);
  private messageService = inject(MessageService);

 expandedOfficeId: number | null = null;

  toggleExpansion(id: number): void {
    this.expandedOfficeId = this.expandedOfficeId === id ? null : id;
  }


  offices$ = this.state.offices$;
  ref: DynamicDialogRef | null = null;

  ngOnInit(): void {
    this.state.loadOffices(); // trigger loading initially

    // Log emitted values
    this.offices$.subscribe(offices => {
      console.log('Loaded offices:', offices);
    });
  }

  getTranslation(office: Office, lang: string): OfficeTranslation | null {
    return office.translations.find(t => t.language === lang) || null;
  }

  openAddOfficeDialog() {
    this.ref = this.dialogService.open(AddOfficeComponent, {
      width: '50%',
     showHeader: false,
     closable: true,
    });

    this.ref.onClose.subscribe((added: boolean) => {
      if (added) this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Office added successfully' });
    });
  }

  openEditTranslationDialog(translation: OfficeTranslation) {
  this.ref = this.dialogService.open(UpdateOfficeTranslationComponent, {
     width: '50%',
     showHeader: false,
     closable: true,
     data: translation,
  });

  this.ref.onClose.subscribe((updated: boolean) => {
    if (updated) this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Translation updated' });
  });
}


  confirmDeleteOffice(id: number) {
    this.confirmService.confirm({
      message: 'Are you sure you want to delete this office?',
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'btn-outline-danger me-2',
      rejectButtonStyleClass: 'btn-outline-primary me-2',
      accept: () => {
        this.state.deleteOffice(id);
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Office deleted'
        });
      }
    });
  }

}
