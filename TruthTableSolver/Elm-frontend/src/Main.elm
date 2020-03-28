module Main exposing (..)
import Browser 
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http exposing (..)



-- MAIN


main =
  Browser.element
    { init = init
    , update = update
    , subscriptions = subscriptions
    , view = view
    }



-- MODEL


type ModelStates
  = Failure Http.Error
  | Loading
  | Unloaded
  | Success String

type alias Model = {
 state : ModelStates,
 expression : String
 }

init : () -> (Model, Cmd Msg)
init _ =
  ( {state = Unloaded, expression = ""}
  , Cmd.none
  )



-- UPDATE


type Msg
  = Received (Result Http.Error String)
  | Requesting
  | Typing String
  | AddSpecialBi
  | AddSpecialCond1
  | AddSpecialCond2
  | AddSpecialXOR


update : Msg -> Model -> (Model, Cmd Msg)
update msg model =
  case msg of
      Requesting ->
        (
        {model | state = Loading},
        Http.get{ 
        url = "/api/TruthTable?expr=" ++ model.expression,
        expect = Http.expectString Received}
        )

      Received result ->
        case result of
             Ok table ->
              ({model | state = Success table}, Cmd.none)
              
             Err err ->
              ({model | state = Failure err}, Cmd.none)

      Typing expr ->
          ({model | expression = expr}, Cmd.none)

      AddSpecialBi ->
          ({model | expression = model.expression ++ "↔"}, Cmd.none)


      AddSpecialCond1 ->
          ({model | expression = model.expression ++ "→"}, Cmd.none)


      AddSpecialCond2 ->
           ({model | expression = model.expression ++ "⊃"}, Cmd.none)


      AddSpecialXOR ->
          ({model | expression = model.expression ++ "⊕"}, Cmd.none)









-- SUBSCRIPTIONS


subscriptions : Model -> Sub Msg
subscriptions model =
  Sub.none



-- VIEW


view : Model -> Html Msg
view model =
  let {state, expression} = model
  in
  case state of
      Failure _ ->
          div [] [
          div [id "extra-symbols"]
          [
            button [onClick AddSpecialBi][text "↔"],
            button [onClick AddSpecialCond1][text "→"],
            button [onClick AddSpecialCond2][text "⊃"],
            button [onClick AddSpecialXOR][text "⊕"]
          ],
          br [] [],
          viewInput "Text" "Please enter an expression..." expression Typing,
          button [onClick Requesting] [text "solve"],
          br [] [],
          br [] [],
          text "error"
          ]

      Success table ->
          div [] [
          div [id "extra-symbols"]
          [
          button [onClick AddSpecialBi][text "↔"],
          button [onClick AddSpecialCond1][text "→"],
          button [onClick AddSpecialCond2][text "⊃"],
          button [onClick AddSpecialXOR][text "⊕"]
          ],
          br [] [],
          viewInput "Text" "Please enter an expression..." expression Typing,
          button [onClick Requesting] [text "solve"],
          br [] [],
          br [] [],
          textarea [] [text table]
          ]

      Loading ->
          div [] [
          div [id "extra-symbols"]
          [
          button [onClick AddSpecialBi][text "↔"],
          button [onClick AddSpecialCond1][text "→"],
          button [onClick AddSpecialCond2][text "⊃"],
          button [onClick AddSpecialXOR][text "⊕"]
          ],
          br [] [],
          viewInput "Text" "Please enter an expression..." expression Typing,
          button [onClick Requesting] [text "solve"],
          br [] [],
          br [] [],
          text "Loading..."
          ]

      Unloaded ->
          div [] [
          div [id "extra-symbols"]
          [
          button [onClick AddSpecialBi][text "↔"],
          button [onClick AddSpecialCond1][text "→"],
          button [onClick AddSpecialCond2][text "⊃"],
          button [onClick AddSpecialXOR][text "⊕"]
          ],
          br [] [],
          viewInput "Text" "Please enter an expression..." expression Typing,
          button [onClick Requesting] [text "solve"]
          ]

viewInput : String -> String -> String -> (String -> msg) -> Html msg
viewInput t p v onIn =
  input [type_ t, placeholder p, value v, onInput onIn] []
