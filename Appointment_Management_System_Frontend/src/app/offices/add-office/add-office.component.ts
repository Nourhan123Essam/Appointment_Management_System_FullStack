import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SUPPORTED_LANGUAGES } from '../../core/constants/languages';
import { OfficeStateService } from '../../core/services/State/office-state.service';
import { OfficeTranslation } from '../../core/Interfaces/OfficeTranslation';
import { Office } from '../../core/Interfaces/Office';

import { TabViewModule } from 'primeng/tabview';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { CommonModule } from '@angular/common';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-add-office',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TabViewModule,
    InputTextModule,
    ButtonModule,
    TableModule,
    DialogModule,
    ConfirmDialogModule,
    ToastModule,
    TooltipModule
  ],
  templateUrl: './add-office.component.html',
  styleUrl: './add-office.component.css'
})
export class AddOfficeComponent implements OnInit {
  officeForm!: FormGroup;
  languages = SUPPORTED_LANGUAGES;

  constructor(
    private fb: FormBuilder,
    private officeState: OfficeStateService,
    public ref: DynamicDialogRef,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.officeForm = this.fb.group({});
    this.languages.forEach(lang => {
      this.officeForm.addControl(
        lang,
        this.fb.group({
          name: ['', Validators.required],
          country: ['', Validators.required],
          state: ['', Validators.required],
          city: ['', Validators.required],
          streetName: ['', Validators.required]
        })
      );
    });
  }

  submit(): void {
    if (this.officeForm.invalid) return;

    const translations = this.languages.map(lang => ({
      id: 0,
      officeID: 0,
      language: lang,
      ...this.officeForm.value[lang]
    }));

    const newOffice: Office = { id: 0, translations };

    this.officeState.createOffice(newOffice)
      .subscribe(success => {
        if(success){
          this.ref.close(success);
        }
        else{
          this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to add office'
        });
        }
      }
    );
  }

  cancel() {
    this.ref.close(false);
  }
}
