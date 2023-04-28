import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '@app/models/identity/User';
import { UserLogin } from '@app/models/identity/UserLogin';
import { UserUpdate } from '@app/models/identity/UserUpdate';
import { environment } from '@environments/environment';
import { Observable, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private currentUserSource = new ReplaySubject<User>(1); // Essa variável "currentUserSource" vai receber diversas atualizações que será passada para todo o sistema
  public currentUser$ = this.currentUserSource.asObservable(); // $ -> significa que é um Observable ou subject

  baseUrl = environment.apiURL + 'api/account/'

constructor(private http: HttpClient) { }

public login(model: any): Observable<void>
{
  return this.http.post<User>(this.baseUrl + 'login', model).pipe(
    take(1),
    map((response: User) => {
      const user = response;
      if (user){
        this.setCurrentUser(user)
      }
    })
  );
}

getUser(): Observable<UserUpdate>{
  return this.http.get<UserUpdate>(this.baseUrl + 'getUser').pipe(take(1));
}

updateUser(model: UserUpdate): Observable<void>{ // eu atualizei ??
  return this.http.put<UserUpdate>(this.baseUrl + 'updateUser', model).pipe( // mando o que eu atualizei para o meu UserUpdate
    take(1), // será chamado 1 vez
    map((user: UserUpdate) => {
      this.setCurrentUser(user)
      }
    )
  );
}

public register(model: any): Observable<void>
{
  return this.http.post<UserLogin>(this.baseUrl + 'register', model).pipe(
    take(1),
    map((response: User) => {
      const user = response;
      if (user){
        this.setCurrentUser(user)
      }
    })
  );
}

logout(): void {
  localStorage.removeItem('user');
  this.currentUserSource.next(null); // Se eu to deslogando o próximo next tem que ser null e automaticamente será deslogado
  //this.currentUserSource.complete();
}

public setCurrentUser(user: User): void{
  localStorage.setItem('user', JSON.stringify(user));
  this.currentUserSource.next(user); // Esse usuário vai ser o que todo mundo que está inscrito no currentUser$ vai receber
}

}
