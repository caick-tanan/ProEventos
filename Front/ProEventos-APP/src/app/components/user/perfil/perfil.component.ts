import { Component, OnInit } from '@angular/core';
import { AccountService } from '@app/Services/account.service';
import { UserUpdate } from '@app/models/identity/UserUpdate';
import { environment } from '@environments/environment';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss']
})
export class PerfilComponent implements OnInit {
  public usuario = {} as UserUpdate;
  public file: File;
  public imageURL = '';

  public get ehPalestrante(): boolean {
    return this.usuario.funcao === 'Palestrante'; // Se for palestrante retorna True
  }

  constructor(private spinner: NgxSpinnerService,
              private toastr: ToastrService,
              private accountService:  AccountService
    ) {}

  ngOnInit(): void {
  }

  public setFormValue(usuario: UserUpdate): void{
    this.usuario = usuario;
    if(this.usuario.imageURL)
      this.imageURL = environment.apiURL + `resources/perfil/${this.usuario.imageURL}`;
    else
      this.imageURL = './assets/img/perfil.png';
  }

  onFileChange(ev: any): void
    {
      const reader = new FileReader();

      reader.onload = (event: any) => this.imageURL = event.target.result; // Isso serve para carregar a imagem escolhida na tela alterando o ícone de nuvem para a imagem selecionado pelo usuário

      this.file = ev.target.files; // Vou atribuir para a minha variável file todos os arquivos que estão no meu input do meu HTML
      reader.readAsDataURL(this.file[0]);

      this.uploadImagem();
    }

    uploadImagem(): void {
      this.spinner.show();
      this.accountService.postUpload(this.file).subscribe(
        () => {
          this.toastr.success('Imagem atualizada com Sucesso', 'Sucesso!');
        },
        (error: any) => {
          this.toastr.error('Erro ao fazer upload de imagem', 'Erro!');
          console.log(error);
        }
      ).add(() => this.spinner.hide());
    }

  // Conveniente para pegar um FormField apenas com a letra F
  get f(): any {
    return ''
  }
}
