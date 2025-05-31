import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SUPPORTED_LANGUAGES } from '../../core/constants/languages';
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
import { SpecializationWithTranslations } from '../../core/Interfaces/SpecializationWithTranslations';
import { SpecializationStateService } from '../../core/services/State/specialization-state.service';

@Component({
  selector: 'app-add-specialist',
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
  templateUrl: './add-specialist.component.html',
  styleUrl: './add-specialist.component.css'
})
export class AddSpecialistComponent {
  languages = SUPPORTED_LANGUAGES;
  specializationForm: FormGroup;
  ref: DynamicDialogRef | null = null;

  private state = inject(SpecializationStateService);

  constructor(private fb: FormBuilder, private dialogRef: DynamicDialogRef) {
    this.specializationForm = this.fb.group({});
    this.languages.forEach(lang => {
      this.specializationForm.addControl(
        lang,
        this.fb.group({
          name: ['', Validators.required]
        })
      );
    });
  }

 submit(): void {
  if (this.specializationForm.invalid) return;

  const translations = this.languages.map(lang => ({
    id: 0,
    specializationId: 0,
    language: lang,
    ...this.specializationForm.value[lang]
  }));

  const newSpesialist: SpecializationWithTranslations = { id: 0, translations };

  this.state.createSpecialization(newSpesialist);
  this.cancel();
}
cancel() {
  this.dialogRef.close();
}

}
