import { NUMBER_TYPE } from '@angular/compiler/src/output/output_ast';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Evento } from '@app/models/Evento';
import { Lote } from '@app/models/Lote';
import { EventoService } from '@app/Services/evento.service';
import { LoteService } from '@app/Services/lote.service';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {
  modalRef: BsModalRef;
  eventoId: number;
  evento = {} as Evento;

  form: FormGroup;
  estadoSalvar = 'post'; // aqui vai iniciar vazio por isso está post = (Adicionar)
  loteAtual = {id: 0, nome: '', indice: 0}

  get modoEditar(): boolean {
    return this.estadoSalvar === 'put';
  }

  get lotes(): FormArray { // o ge Lotes() é o item que está dentro do meu formulário que é o meu form principal
    return this.form.get('lotes') as FormArray; // e falo que vai ser um Array
  }

  get f(): any{
    return this.form.controls;
  }

  get bsConfig(): any {
    return{
      isAnimated: true,
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY hh:mm a',
      containerClass: 'theme-default',
      showWeekNumbers: false
    };
  }

  get bsConfigLote(): any {
    return{
      isAnimated: true,
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY',
      containerClass: 'theme-default',
      showWeekNumbers: false
    };
  }

  constructor(private fb: FormBuilder,
              private localeService: BsLocaleService,
              private activatedRouter: ActivatedRoute,
              private eventoService: EventoService,
              private spinner: NgxSpinnerService,
              private toastr: ToastrService,
              private modalService: BsModalService,
              private loteService: LoteService,
              private router: Router
    )
    {
      this.localeService.use('pt-br');
    }

    public carregarEvento(): void {
      this.eventoId = +this.activatedRouter.snapshot.paramMap.get('id'); // o + serve para passar de number para string

      if (this.eventoId !== null && this.eventoId !== 0){
        this.spinner.show();

        this.estadoSalvar = 'put'; // aqui vai pegar as informações por isso está put = (Atualizar)

        this.eventoService.getEventoById(this.eventoId).subscribe( // O + converte para inteiro ou string
        (evento: Evento) => {
          this.evento  = {...evento}; // Ele vai pegar todos os itens de evento e atribuir para o this.evento
          this.form.patchValue(this.evento);
          this.carregarLotes(); // aqui vai carregar as informações dos lotes que já foram preenchidas conforme o método criado
        },
        (error: any) => {
          this.spinner.hide();
          this.toastr.error('Erro ao tentar carregar Evento.', 'Erro!');
          console.error(error);
        },
        () => this.spinner.hide(),
        );
      }
    }

    public carregarLotes(): void
    {
      this.loteService.getLotesByEventoId(this.eventoId).subscribe(
        (lotesRetorno: Lote[]) => {
          lotesRetorno.forEach(lote => {
            this.lotes.push(this.criarLote(lote));
          });
        },
        (error: any) => {
          this.toastr.error('Erro ao tentar carregar lotes', 'Erro');
          console.error(error);
        }
      ).add(() => this.spinner.hide());
    }

    ngOnInit(): void {
      this.carregarEvento();
      this.validation();
    }

    public validation(): void {
      this.form = this.fb.group({
        tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
        local: ['', Validators.required],
        dataEvento: ['', Validators.required],
        qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
        imageURL: ['', Validators.required],
        email: ['',
        [Validators.required, Validators.email]],
        telefone: ['', Validators.required],
        // tslint:disable-next-line: max-line-length
        lotes: this.fb.array([]) // Isso serve para criar um array de lotes, pois toda vez que eu clicar em adicionar ele tem que adicionar outro lote
      });
    }

    adicionarLote(): void { // Push -> criar dentro desse form um agrupamento
      // tslint:disable-next-line: max-line-length
      this.lotes.push(this.criarLote({id: 0} as Lote)); // {id: 0} as Lote irá criar um novo lote com os valores padrões definidos a baixo em criar lote
    }

    // tslint:disable-next-line: max-line-length
    criarLote(lote: Lote): FormGroup { // vai criar o formato do lote com suas validações individualmente ou seja cada novo lote será diferente com os campos a baixo
      return this.fb.group({
        id: [lote.id],
        nome: [lote.nome, Validators.required],
        quantidade: [lote.quantidade, Validators.required],
        preco: [lote.preco, Validators.required],
        dataInicio: [lote.dataInicio],
        dataFim: [lote.dataFim]
      });
    }

    public removerLote(template: TemplateRef<any>,
                      indice: number): void {
      this.loteAtual.id = this.lotes.get(indice + '.id').value;
      this.loteAtual.nome = this.lotes.get(indice + '.nome').value;
      this.loteAtual.indice = indice;


      this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
    }

    confirmDelLote(): void
    {
      this.modalRef.hide();
      this.spinner.show();

      this.loteService.deleteLote(this.eventoId, this.loteAtual.id)
      .subscribe(
        () => {
          this.toastr.success('Lote deletado com sucesso', 'Sucesso');
          this.lotes.removeAt(this.loteAtual.indice);
        },
        (error: any) => {
          this.toastr.error('Erro ao tentar deletar o lote ${this.loteAtual.id}', 'Erro');
          console.error(error);
        }
      ).add(() => this.spinner.hide());

    }

    public retornaTituloLote(nome: string): string{
      return nome === null || nome === '' ? 'Nome do lote' : nome;
    }

    declineDelLote(): void {
      this.modalRef.hide();
    }

    public resetForm(): void {
      this.form.reset();
    }

    public cssValidator(campoForm: FormControl | AbstractControl | null): any{
      return {'is-invalid': campoForm.errors && campoForm.touched};
    }

    public salvarEvento(): void {
      this.spinner.show();
      if (this.form.valid) {

        this.evento = (this.estadoSalvar === 'post')
        ? {...this.form.value}
        : {id: this.evento.id, ...this.form.value};

        this.eventoService[this.estadoSalvar](this.evento).subscribe(
          (eventoRetorno: Evento) => {
            this.toastr.success('Evento salvo com Sucesso!', 'Sucesso');
            // tslint:disable-next-line: max-line-length
            this.router.navigate([`eventos/detalhe/${eventoRetorno.id}`]); // Vai redirecionar para que quando crie um evento depois de clicar em salvar apareça os lotes para serem adicionados
          },
          (error: any) => {
            console.error(error);
            this.spinner.hide();
            this.toastr.error('Error ao salvar evento', 'Erro');
          },
          () => this.spinner.hide()
          );
      }
    }

    public salvarLotes(): void {
      if (this.form.controls.lotes.valid){
        this.spinner.show();
        this.loteService.saveLote(this.eventoId, this.form.value.lotes)
          .subscribe(
            () => {
              this.toastr.success('Lotes salvos com Sucesso!', 'Sucesso!');
          },
          (error: any) => {
            this.toastr.error('Erro ao tentar salvar lotes.', 'Erro');
            console.error(error);
          }
          ).add(() => this.spinner.hide());
      }
    }
}

