import { useNavigate } from "react-router-dom";
import { useTodoGroupStore } from "./todoGroupStore";
import { Card, Descriptions, Button } from "antd";

export const TodoGroupView = () => {
  const { currentTodoGroup } = useTodoGroupStore();
  const navigate = useNavigate();
  if (!currentTodoGroup) return null;

  return (
    <Card
      title="Todo List Details"
      extra={
        <Button type="default" onClick={() => navigate("/todo-list")}>
          Back
        </Button>
      }
    >
      <Descriptions column={1} bordered>
        <Descriptions.Item label="Name">
          {currentTodoGroup.title}
        </Descriptions.Item>
        <Descriptions.Item label="Colour">
          <div
            style={{
              display: "inline-block",
              width: 24,
              height: 24,
              borderRadius: "50%",
              backgroundColor: currentTodoGroup.colour,
              border: "1px solid #ccc",
            }}
          />
        </Descriptions.Item>
      </Descriptions>
    </Card>
  );
};
