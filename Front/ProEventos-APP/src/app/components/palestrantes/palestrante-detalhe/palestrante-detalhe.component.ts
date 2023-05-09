import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PalestranteService } from '@app/Services/palestrante.service';
import { Palestrante } from '@app/models/Palestrante';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { debounceTime, map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-palestrante-detalhe',
  templateUrl: './palestrante-detalhe.component.html',
  styleUrls: ['./palestrante-detalhe.component.scss']
})
export class PalestranteDetalheComponent implements OnInit {

  form!: FormGroup;
  public situacaoDoForm = '';
  public corDaDescricao = '';

  constructor( private fb: FormBuilder,
    public palestranteService: PalestranteService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService) { }

  ngOnInit() {
    this.validation();
    this.verificaForm();
    this.carregarPalestrante()
  }

  private validation(): void {
    this.form = this.fb.group({
      miniCurriculo: [''],
    });
  }

  private carregarPalestrante(): void {
    this.spinner.show();

    this.palestranteService
      .getPalestrante()
      .subscribe(
        (palestrante: Palestrante) => {
          this.form.patchValue(palestrante);
        },
        (error: any) => {
          this.toastr.error('Erro ao Carregar o Palestrante', 'Erro')
        }
      )
  }

  public get f(): any {
    return this.form.controls;
  }

  private verificaForm(): void {
    this.form.valueChanges
      .pipe(
        map(() => { // A medida que for digitando aparecerá essa mensagem
          this.situacaoDoForm = 'Minicurrículo está sendo Atualizado!';
          this.corDaDescricao = 'text-warning';
        }),
        debounceTime(1000), // Segura o estado do observable ate que pare de ser att. por 1 seg aqui no caso
        tap(() => this.spinner.show())
      )
      .subscribe(() => { // Como estou inscrito, o subscribe fica "escutando" qualquer alteração
        this.palestranteService
          .put({...this.form.value }) // Como o usuário já está logado, pegará o Token do usuário
          .subscribe(
            () => {
              this.situacaoDoForm = 'Minicurrículo foi ataulizado!';
              this.corDaDescricao = 'text-success';

              setTimeout(() => {
                this.situacaoDoForm = 'Minicurrículo foi carregado!';
                this.corDaDescricao = 'text-muted';
              }, 2000);
            },
            () => {
              this.toastr.error('Erro ao tentar atualizar Palestrante', 'Erro');
            }
          )
          .add(() => this.spinner.hide())
      });

    }
}
